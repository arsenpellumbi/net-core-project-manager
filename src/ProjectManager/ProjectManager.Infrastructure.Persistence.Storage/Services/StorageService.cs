using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Configuration;
using MimeMapping;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Services
{
    public abstract class StorageService
    {
        internal readonly string[] AllowedExtensions =  { ".pdf", ".image", ".cargoimage", ".webm", ".mp4", ".mpeg", ".mp3", ".wav", ".dcs", ".dicos", ".xlsx", ".xls", ".docx",
            ".ppt", ".pptx", ".zip", ".xml", ".html", ".css", ".js", ".json", ".woff", ".uff"};

        internal readonly string[] NonZipExtensions = { ".ppt", ".pptx", ".xlsx", ".xls", ".docx" };

        protected readonly StorageConfig StorageConfig;

        protected StorageService(StorageConfig storageConfig)
        {
            StorageConfig = storageConfig;
        }

        internal bool IsZip(byte[] fileBytes, string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);
            if (NonZipExtensions.Contains(fileExtension))
                return false;

            try
            {
                using var stream = new MemoryStream(fileBytes);
                using var archive = new ZipArchive(stream);
                return archive.Entries.Count > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal byte[] ZipEntryToByteArray(ZipArchiveEntry entry)
        {
            using var fileStream = entry.Open();
            using var ms = new MemoryStream();
            fileStream.CopyTo(ms);

            return ms.ToArray();
        }

        /// <summary>
        /// Generated and returns an url of the file inside the default container specified in appsettings
        /// </summary>
        /// <param name="pathInContainer">Path of file inside the container</param>
        /// <returns>An url</returns>
        public string GetUrl(string pathInContainer) => GetUrl(pathInContainer, StorageConfig.Container);

        /// <summary>
        /// Generated and returns an url of the file inside the container
        /// </summary>
        /// <param name="pathInContainer">Path of file inside the container</param>
        /// <param name="container">Name of the container name containing the file. If not specified, the default container is the one specified in appsettings</param>
        /// <returns>An url</returns>
        private string GetUrl(string pathInContainer, string container)
        {
            if (string.IsNullOrEmpty(pathInContainer) || pathInContainer.StartsWith($"{StorageConfig.StorageUrl}/{container}/"))
                return pathInContainer;

            return Uri.EscapeUriString($"{StorageConfig.StorageUrl}/{container}/{pathInContainer}");
        }

        protected string GetContentType(string fileName)
        {
            if (!fileName.EndsWith(@"/"))
            {
                return MimeUtility.GetMimeMapping(fileName);
            }

            return MimeUtility.UnknownMimeType;
        }
    }
}