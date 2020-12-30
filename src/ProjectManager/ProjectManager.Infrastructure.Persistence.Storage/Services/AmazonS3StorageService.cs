using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Configuration;
using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Exceptions;
using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using File = AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models.File;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Services
{
    public sealed class AmazonS3StorageService : StorageService, IStorageService
    {
        private readonly Configuration.AmazonS3Config _amazonS3Config;
        private readonly IAmazonS3 _amazonS3;

        public AmazonS3StorageService(StorageConfig storageConfig) : base(storageConfig)
        {
            _amazonS3Config = storageConfig.AmazonS3Config;

            var regionEndpoint = RegionEndpoint.EnumerableAllRegions.FirstOrDefault(x => x.SystemName == storageConfig.AmazonS3Config.SystemName);
            if (regionEndpoint == null)
                throw new StorageException("");

            _amazonS3 = new AmazonS3Client(storageConfig.AmazonS3Config.AwsAccessKeyId,
                storageConfig.AmazonS3Config.AwsSecretAccessKey,
                regionEndpoint);
        }

        public async Task<Folder> CreateFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            var newFolderKey = GetAbsolutePath(path);

            var response = await ListContainerObjects(newFolderKey);
            if (response.S3Objects.Count > 0)
            {
                throw new StorageException($"Folder with key {newFolderKey} exists!");
            }

            await CreateEmptyFolder(newFolderKey);

            return await GetFolder(path);
        }

        public Task UpdateFolder(string path, Folder folder)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteFolder(string path)
        {
            var folderKey = GetAbsoluteTrashPath(path);

            var trashFileKeys = await GetTrashFolderContentKeys(folderKey);

            await DeleteFiles(trashFileKeys);
        }

        public async Task<Folder> GetFolder(string path = null)
        {
            var folderKey = GetAbsolutePath(path);
            var response = await ListContainerObjects(folderKey);
            var current = ToFolder(folderKey);

            foreach (var commonPrefix in response.CommonPrefixes)
            {
                current.Children.Add(ToFolder(commonPrefix));
            }

            foreach (var file in response.S3Objects.Where(x => !x.Key.EndsWith(@"/")))
            {
                current.Files.Add(ToFile(file));
            }

            return current;
        }

        public async Task CreateFiles(IEnumerable<FileContent> files)
        {
            foreach (var file in files)
            {
                var cloudPath = new CloudPath(file.Path, file.Filename);

                if (IsZip(file.FileBytes, file.Filename))
                {
                    var filenamePart = file.Filename.Split('.');
                    var newParts = filenamePart.Take(filenamePart.Count() - 1).ToList();
                    var zipFolder = string.Join(".", newParts);

                    var newFolderKey = string.IsNullOrWhiteSpace(cloudPath.Path) ? zipFolder : $"{cloudPath.Path}/{zipFolder}";
                    var folder = await CreateFolder(newFolderKey);

                    using (var stream = new MemoryStream(file.FileBytes))
                    {
                        using (var archive = new ZipArchive(stream))
                        {
                            foreach (var entry in archive.Entries)
                            {
                                if (entry.Length > 0)
                                {
                                    var fileContent = new FileContent
                                    {
                                        ContentLength = (int)entry.Length,
                                        FileBytes = ZipEntryToByteArray(entry),
                                        Filename = entry.Name,
                                        ContentType = GetContentType(entry.Name),
                                        Path = $"{folder.Path}/{folder.Name}"
                                    };

                                    var extension = Path.GetExtension(fileContent.Filename);

                                    if (!AllowedExtensions.Contains(extension) && !fileContent.ContentType.Contains("image"))
                                        throw new StorageException("Invalid file(s). Note that allowed type files are all image types, video type mp4, webm, ogg, audio types mp3, ogg, wav, pdf, xlsx, xls, docx, ppt, pptx, zip, xml, html, css, js, json, woff and dicos files.");

                                    if (entry.FullName.Length > entry.Name.Length)
                                    {
                                        fileContent.Path = $"{folder.Path}/{folder.Name}/{entry.FullName.Substring(0, entry.FullName.Length - entry.Name.Length)}";
                                    }

                                    await CreateFile(fileContent);
                                }
                            }
                        }
                    }
                }
                else
                {
                    await CreateFile(file);
                }
            }
        }

        public async Task CreateFile(FileContent file)
        {
            var cloudPath = new CloudPath(file.Path, file.Filename);
            var fileAbsolutePath = GetAbsolutePath(cloudPath.TargetPath);

            var extension = Path.GetExtension(file.Filename);
            if (!AllowedExtensions.Contains(extension) && !file.ContentType.Contains("image"))
                throw new StorageException("Invalid file(s). Note that allowed type files are all image types, video type mp4, webm, ogg, audio types mp3, ogg, wav, pdf, xlsx, xls, docx, ppt, pptx, zip, xml, html, css, js, json, woff and dicos files.");

            var folderKey = GetAbsolutePath(cloudPath.Path);

            var response = await ListContainerObjects(folderKey);
            var newFileKey = GetNewFilePathIfExists(response.S3Objects.Select(x => x.Key), fileAbsolutePath, fileAbsolutePath);

            var putRequest = new PutObjectRequest
            {
                BucketName = _amazonS3Config.Bucket,
                Key = newFileKey,
                InputStream = new MemoryStream(file.FileBytes)
            };

            await _amazonS3.PutObjectAsync(putRequest);
        }

        public Task UpdateFile(string path, File file)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetFile(string path)
        {
            var fileKey = GetAbsolutePath(path);

            using (var stream = await _amazonS3.GetObjectStreamAsync(_amazonS3Config.Bucket, fileKey, null))
            {
                if (stream.Length < 1)
                    throw new StorageException("File not found.");

                using (var memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
        }

        public async Task BulkDelete(FolderContent content)
        {
            var trashFileKeys = new List<string>();

            foreach (var folder in content.Folders)
            {
                trashFileKeys.AddRange(await GetTrashFolderContentKeys(folder.DeletedName));
            }

            trashFileKeys.AddRange(content.Files.Select(x => x.DeletedName));

            var keys = new List<Guid>();
            keys.AddRange(content.Folders.Select(x => Guid.Parse(x.DeletedName.Remove(x.DeletedName.LastIndexOf("/")).Split("/").Last())));
            keys.AddRange(content.Files.Select(x => Guid.Parse(x.DeletedName.Split("/").Last())));

            await DeleteFiles(trashFileKeys);
        }

        public async Task Copy(string path, FolderContent content)
        {
            await Copy(path, content, false);
        }

        public async Task Move(string path, FolderContent content)
        {
            await Copy(path, content, true);
        }

        private async Task<ListObjectsV2Response> ListContainerObjects(string folderKey, bool useDelimiter = true)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _amazonS3Config.Bucket,
                Prefix = $"{folderKey}/",
                Delimiter = useDelimiter ? "/" : string.Empty
            };

            var response = new ListObjectsV2Response();

            var paginatedResponse = new ListObjectsV2Response
            {
                IsTruncated = true
            };

            while (paginatedResponse.IsTruncated)
            {
                request.ContinuationToken = paginatedResponse.NextContinuationToken;
                paginatedResponse = await _amazonS3.ListObjectsV2Async(request);

                response.S3Objects.AddRange(paginatedResponse.S3Objects);
                response.CommonPrefixes.AddRange(paginatedResponse.CommonPrefixes);
            }

            return response;
        }

        private Folder ToFolder(string folderKey)
        {
            var isMainContainer = folderKey == GetAbsolutePath();
            var cloudPath = new CloudPath(isMainContainer ? "/" : folderKey.Substring(GetAbsolutePath().Length));
            return new Folder
            {
                Name = isMainContainer || cloudPath.Target == null ? "" : cloudPath.Target,
                Path = isMainContainer ? null : $"/{cloudPath.Path}"
            };
        }

        private File ToFile(S3Object file)
        {
            var cloudPath = new CloudPath(file.Key.Substring(GetAbsolutePath().Length));
            return new File
            {
                Created = file?.LastModified ?? new DateTime(2017, 1, 1),
                ContentType = GetContentType(file.Key),
                Edited = file?.LastModified,
                Name = cloudPath.Target,
                Path = $"/{cloudPath.Path}/".Replace("//", "/"),
                Size = file.Size,
                Url = GetS3PreSignedUrl(cloudPath.TargetPath),
            };
        }

        private async Task<string> GetNewDirectoryPathIfExists(string folderKey, string folderKeyToCheck, int number = 0)
        {
            ListObjectsV2Response response = await ListContainerObjects(folderKeyToCheck);

            if (response.S3Objects.Count <= 0)
            {
                return folderKeyToCheck;
            }
            return await GetNewDirectoryPathIfExists(folderKey, $"{folderKey} - Copy {(number > 1 ? $"({number + 1})" : "")}", number + 1);
        }

        private async Task Copy(string path, FolderContent content, bool postDelete)
        {
            var filesToCopy = new List<(File File, S3Object S3Object, string DestinationKey)>();

            foreach (var folder in content.Folders)
            {
                var originalPath = new CloudPath(folder.Path, folder.Name);

                var newFolderKey = $"{GetAbsolutePath(path)}/{folder.Name}";
                var destinationFolderKey = await GetNewDirectoryPathIfExists(newFolderKey, newFolderKey);

                var folderKey = GetAbsolutePath(originalPath.TargetPath);

                var files = await GetInnerFolderFiles(folderKey);
                filesToCopy.AddRange(files.Select(x => (x.File, x.S3Object, $"{destinationFolderKey}{x.S3Object.Key.Substring(folderKey.Length)}")));
            }

            if (content.Files.Any())
            {
                var response = await ListContainerObjects(GetAbsolutePath(path));
                filesToCopy.AddRange(content.Files.Select(x => (x, new S3Object { Key = GetAbsolutePath(x.Path) }, GetNewFilePathIfExists(response.S3Objects.Select(x => x.Key), $"{GetAbsolutePath(path)}/{x.Name}", $"{GetAbsolutePath(path)}/{x.Name}"))));
            }

            await CopyFile(filesToCopy, postDelete);
        }

        private async Task CopyFile(IEnumerable<(File File, S3Object S3Object, string DestinationKey)> filesToCopy, bool postDelete)
        {
            var deleteRequest = new DeleteObjectsRequest
            {
                BucketName = _amazonS3Config.Bucket
            };

            foreach (var fileToCopy in filesToCopy)
            {
                var copyRequest = new CopyObjectRequest()
                {
                    SourceBucket = _amazonS3Config.Bucket,
                    DestinationBucket = _amazonS3Config.Bucket,
                    SourceKey = GetAbsolutePath(fileToCopy.S3Object.Key),
                    DestinationKey = fileToCopy.DestinationKey
                };

                await _amazonS3.CopyObjectAsync(copyRequest);

                if (postDelete)
                {
                    deleteRequest.AddKey(fileToCopy.S3Object.Key);
                }
            }
        }

        private async Task<List<(File File, S3Object S3Object)>> GetInnerFolderFiles(string absolutePath)
        {
            var fileList = new List<(File File, S3Object S3Object)>();

            var response = await ListContainerObjects(absolutePath, false);
            foreach (var file in response.S3Objects)
            {
                fileList.Add((ToFile(file), file));
            }

            return fileList;
        }

        private async Task CreateEmptyFolder(string folderKey)
        {
            folderKey = $"{folderKey}/";
            if (await S3ObjectExists(folderKey)) return;

            var putRequest = new PutObjectRequest()
            {
                BucketName = _amazonS3Config.Bucket,
                Key = folderKey,
                InputStream = new MemoryStream()
            };

            await _amazonS3.PutObjectAsync(putRequest);
        }

        private async Task<bool> S3ObjectExists(string key)
        {
            try
            {
                await _amazonS3.GetObjectAsync(_amazonS3Config.Bucket, key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<IList<string>> GetTrashFolderContentKeys(string folderKey)
        {
            if (string.IsNullOrWhiteSpace(folderKey) || folderKey == "/" || folderKey == GetAbsoluteTrashPath())
                throw new StorageException("Cannot delete root folder.");

            return await _amazonS3.GetAllObjectKeysAsync(_amazonS3Config.Bucket, $"{folderKey}", null);
        }

        private async Task DeleteFiles(IEnumerable<string> fileKeys)
        {
            var deleteRequest = new DeleteObjectsRequest
            {
                BucketName = _amazonS3Config.Bucket
            };

            foreach (var key in fileKeys)
            {
                deleteRequest.AddKey(key);
            }

            await _amazonS3.DeleteObjectsAsync(deleteRequest);
        }

        public async Task DeleteFile(string path)
        {
            var deleteRequest = new DeleteObjectsRequest
            {
                BucketName = _amazonS3Config.Bucket
            };

            deleteRequest.AddKey(path);

            await _amazonS3.DeleteObjectsAsync(deleteRequest);
        }

        private string GetAbsolutePath(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
                return $"{StorageConfig.Container}";
            return $"{StorageConfig.Container}{(path.StartsWith("/") ? "" : "/")}{path}";
        }

        private string GetAbsoluteTrashPath(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
                return $"{StorageConfig.TrashContainer}";
            return $"{StorageConfig.TrashContainer}{(path.StartsWith("/") ? "" : "/")}{path}";
        }

        private static string GetNewFilePathIfExists(IEnumerable<string> existingFiles, string fileKey, string fileKeyToCheck, int number = 0)
        {
            if (!existingFiles.Contains(fileKeyToCheck))
                return fileKeyToCheck;

            string fileName = fileKey.Substring(0, fileKey.LastIndexOf('.'));
            string fileExtension = fileKey.Substring(fileKey.LastIndexOf('.'));
            return GetNewFilePathIfExists(existingFiles, fileKey, $"{fileName} - Copy {(number > 1 ? $"({number + 1})" : "")}{fileExtension}", number + 1);
        }

        public string GetS3PreSignedUrl(string fileKey)
        {
            return _amazonS3.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = StorageConfig.AmazonS3Config.Bucket,
                Key = fileKey,
                Expires = DateTime.Now.AddHours(StorageConfig.AmazonS3Config.UrlValidTimeInHours)
            });
        }
    }
}