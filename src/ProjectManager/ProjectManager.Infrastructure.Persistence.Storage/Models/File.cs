using System;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models
{
    public class File
    {
        public int FolderId { get; set; }

        public string Name { get; set; }

        public string DeletedName { get; set; }

        public double Size { get; set; }

        public string Path { get; set; }

        public string Url { get; set; }

        public string ContentType { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Edited { get; set; }

        public Folder Folder { get; set; }
    }
}