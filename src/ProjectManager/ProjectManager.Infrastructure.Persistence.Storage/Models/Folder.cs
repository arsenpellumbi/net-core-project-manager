using System;
using System.Collections.Generic;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Models
{
    public class Folder
    {
        public string Name { get; set; }

        public string DeletedName { get; set; }

        public int? ParentId { get; set; }

        public string Path { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Edited { get; set; }

        public Folder? Parent { get; set; }

        public ICollection<Folder> Children { get; set; }

        public ICollection<File> Files { get; set; }
    }
}