using ProjectManager.Core.SeedWork.Domain;
using System;
using System.Collections.Generic;

namespace ProjectManager.Core.Domain
{
    public sealed class Project : BaseEntity, IAggregateRoot
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; }
    }
}