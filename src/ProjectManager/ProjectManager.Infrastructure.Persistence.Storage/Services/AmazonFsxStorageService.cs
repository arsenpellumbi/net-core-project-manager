using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Configuration;
using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Exceptions;
using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using File = AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models.File;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Services
{
    public sealed class AmazonFsxStorageService : StorageService, IStorageService
    {
        private readonly AmazonFsxConfig _amazonFsxConfig;

        public AmazonFsxStorageService(StorageConfig storageConfig) : base(storageConfig)
        {
            _amazonFsxConfig = storageConfig.AmazonFsxConfig;
        }

        public async Task<Folder> CreateFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException(nameof(path));

            var directory = GetAbsolutePath(path);

            if (Directory.Exists(directory))
            {
                throw new StorageException("Folder already exists.");
            }

            Directory.CreateDirectory(directory);

            return await GetFolder(path);
        }

        public Task UpdateFolder(string path, Folder folder)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteFolder(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
                throw new StorageException("Cannot delete root folder.");

            await Task.Run(() =>
            {
                var absolutePath = GetAbsolutePath(path);
                Directory.Delete(absolutePath, true);
            });
        }

        public async Task<Folder> GetFolder(string path = null)
        {
            var absolutePath = GetAbsolutePath(path);

            var current = ToFolder(absolutePath);
            var folderPaths = Directory.GetDirectories(absolutePath);
            foreach (var folderPath in folderPaths)
            {
                current.Children.Add(ToFolder(folderPath));
            }

            var filePaths = Directory.GetFiles(absolutePath);
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                var file = ToFile(filePath, fileInfo);
                current.Files.Add(file);
            }

            return current;
        }

        public async Task CreateFiles(IEnumerable<FileContent> files)
        {
            foreach (var file in files)
            {
                var path = new CloudPath(file.Path, file.Filename);

                if (IsZip(file.FileBytes, file.Filename))
                {
                    var filenamePart = file.Filename.Split('.');
                    var newParts = filenamePart.Take(filenamePart.Count() - 1).ToList();
                    var zipFolder = string.Join(".", newParts);

                    var directory = string.IsNullOrWhiteSpace(path.Path) ? zipFolder : $"{path.Path}/{zipFolder}";
                    var folder = await CreateFolder(directory);

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
            var path = new CloudPath(file.Path, file.Filename);
            var fileAbsolutePath = GetAbsolutePath(path.TargetPath);

            var extension = Path.GetExtension(file.Filename);
            if (!AllowedExtensions.Contains(extension) && !file.ContentType.Contains("image"))
                throw new StorageException("Invalid file(s). Note that allowed type files are all image types, video type mp4, webm, ogg, audio types mp3, ogg, wav, pdf, xlsx, xls, docx, ppt, pptx, zip, xml, html, css, js, json, woff and dicos files.");

            var newFilePath = GetNewFilePathIfExists(fileAbsolutePath, fileAbsolutePath);
            await Task.Run(() =>
            {
                System.IO.File.WriteAllBytes(newFilePath, file.FileBytes);
            });
        }

        public Task UpdateFile(string path, File file)
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]> GetFile(string path)
        {
            var filePath = GetAbsolutePath(path);

            if (!System.IO.File.Exists(filePath))
            {
                throw new StorageException("File not found.");
            }

            using (var fsSource = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                // Read the source file into a byte array.
                byte[] data = new byte[fsSource.Length];
                await fsSource.ReadAsync(data, 0, (int)fsSource.Length);
                return data;
            }
        }

        public async Task BulkDelete(FolderContent content)
        {
            var keys = new List<Guid>();

            foreach (var folder in content.Folders)
            {
                var cloudPath = new CloudPath(folder.Path, folder.DeletedName);
                keys.AddRange(content.Folders.Select(x => Guid.Parse(cloudPath.TargetPath)));
                await DeleteFolder(cloudPath.TargetPath);
            }

            foreach (var file in content.Files)
            {
                var cloudPath = new CloudPath(file.Path, file.DeletedName);
                keys.AddRange(content.Folders.Select(x => Guid.Parse(cloudPath.TargetPath)));
                await DeleteFile(cloudPath.TargetPath);
            }
        }

        public async Task Copy(string path, FolderContent content)
        {
            await Copy(path, content, false);
        }

        public async Task Move(string path, FolderContent content)
        {
            await Copy(path, content, true);
        }

        private Folder ToFolder(string directory)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var isMainContainer = directory == GetAbsolutePath();
            var directoryPath = new CloudPath(isMainContainer ? "/" : directory.Substring(GetAbsolutePath().Length));
            return new Folder
            {
                Created = directoryInfo.CreationTime,
                Edited = directoryInfo.LastWriteTime,
                Name = isMainContainer || directoryPath.Target == null ? "" : directoryPath.Target,
                Path = isMainContainer ? null : $"/{directoryPath.Path}"
            };
        }

        private File ToFile(string directory, FileInfo fileInfo)
        {
            var filePath = new CloudPath(directory.Substring(GetAbsolutePath().Length));
            return new File
            {
                Created = fileInfo?.CreationTime ?? new DateTime(2017, 1, 1),
                ContentType = GetContentType(directory),
                Edited = fileInfo?.LastWriteTime,
                Name = filePath.Target,
                Path = $"/{filePath.Path}/".Replace("//", "/"),
                Size = fileInfo.Length,
                Url = GetUrl(filePath.TargetPath)
            };
        }

        public async Task DeleteFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || path == "/")
            {
                throw new StorageException("Cannot delete root folder.");
            }

            await Task.Run(() =>
            {
                var deletedPath = GetAbsolutePath(path);
                System.IO.File.Delete(deletedPath);
            });
        }

        private async Task Copy(string path, FolderContent content, bool postDelete)
        {
            foreach (var folder in content.Folders)
            {
                var deleted = 0;
                var originalPath = new CloudPath(folder.Path, folder.Name);

                var absolutePath = $"{GetAbsolutePath(path)}/{folder.Name}";
                var toDirectory = GetNewDirectoryPathIfExists(absolutePath, absolutePath);

                Directory.CreateDirectory(toDirectory);

                var directory = GetAbsolutePath(originalPath.TargetPath);
                var directoryContent = GetInnerFolderFiles(directory);

                foreach (var fileTuple in directoryContent)
                {
                    Directory.CreateDirectory($"{toDirectory}{fileTuple.FileInfo.DirectoryName.Substring(directory.Length)}");
                    var toPath = $"{toDirectory}{fileTuple.FileInfo.FullName.Substring(directory.Length)}";
                    await Task.Run(() =>
                    {
                        if (postDelete)
                            System.IO.File.Move(fileTuple.FileInfo.FullName, toPath);
                        else
                            System.IO.File.Copy(fileTuple.FileInfo.FullName, toPath);

                        deleted++;
                    });
                }

                if (postDelete && deleted == directoryContent.Count)
                    Directory.Delete(directory, true);
            }

            foreach (var file in content.Files)
            {
                var cloudPath = new CloudPath(file.Path, file.Name);
                var filePath = GetAbsolutePath(cloudPath.TargetPath);
                var toPath = GetNewFilePathIfExists($"{GetAbsolutePath(path)}/{file.Name}", $"{GetAbsolutePath(path)}/{file.Name}");

                await Task.Run(() =>
                {
                    if (postDelete)
                        System.IO.File.Move(filePath, toPath);
                    else
                        System.IO.File.Copy(filePath, toPath);
                });
            }
        }

        private List<(File File, FileInfo FileInfo)> GetInnerFolderFiles(string absolutePath)
        {
            var fileList = new List<(File File, FileInfo FileInfo)>();

            var filePaths = Directory.GetFiles(absolutePath);
            foreach (var filePath in filePaths)
            {
                var fileInfo = new FileInfo(filePath);
                var file = ToFile(filePath, fileInfo);
                fileList.Add((file, fileInfo));
            }

            var folderPaths = Directory.GetDirectories(absolutePath);
            foreach (var folderPath in folderPaths)
            {
                fileList.AddRange(GetInnerFolderFiles(folderPath));
            }

            return fileList;
        }

        private string GetAbsolutePath(string path = null)
        {
            if (path != null)
                return Path.Combine(Directory.GetCurrentDirectory(), _amazonFsxConfig.SharedFolderNetworkPath, StorageConfig.Container, path);
            return Path.Combine(Directory.GetCurrentDirectory(), _amazonFsxConfig.SharedFolderNetworkPath, StorageConfig.Container);
        }

        private string GetAbsoluteTrashPath(string path = null)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), _amazonFsxConfig.SharedFolderNetworkPath, StorageConfig.TrashContainer, path);
        }

        private static string GetNewFilePathIfExists(string filePath, string filePathToCheck, int number = 0)
        {
            if (!System.IO.File.Exists(filePathToCheck))
                return filePathToCheck;

            var fileName = filePath.Substring(0, filePath.LastIndexOf('.'));
            var fileExtension = filePath.Substring(filePath.LastIndexOf('.'));
            return GetNewFilePathIfExists(filePath, $"{fileName} - Copy {(number > 1 ? $"({number + 1})" : "")}{fileExtension}", number + 1);
        }

        private static string GetNewDirectoryPathIfExists(string directoryPath, string directoryPathToCheck, int number = 0)
        {
            if (!Directory.Exists(directoryPathToCheck))
                return directoryPathToCheck;
            return GetNewDirectoryPathIfExists(directoryPath, $"{directoryPath} - Copy {(number > 1 ? $"({number + 1})" : "")}", number + 1);
        }
    }
}