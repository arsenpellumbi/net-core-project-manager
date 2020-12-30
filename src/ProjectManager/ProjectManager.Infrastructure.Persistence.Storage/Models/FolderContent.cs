using System.Collections.Generic;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models
{
    public class FolderContent
    {
        public IEnumerable<Folder> Folders { get; set; }

        public IEnumerable<File> Files { get; set; }
    }
}