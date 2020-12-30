using System.Collections.Generic;
using System.Threading.Tasks;
using AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Services
{
    public interface IStorageService
    {
        Task<Folder> CreateFolder(string path);

        Task UpdateFolder(string path, Folder folder);

        Task DeleteFolder(string path);

        Task<Folder> GetFolder(string path = null);

        Task CreateFiles(IEnumerable<FileContent> files);

        Task CreateFile(FileContent file);

        Task UpdateFile(string path, File file);

        Task DeleteFile(string path);

        Task<byte[]> GetFile(string path);

        Task BulkDelete(FolderContent content);

        Task Copy(string path, FolderContent content);

        Task Move(string path, FolderContent content);
    }
}