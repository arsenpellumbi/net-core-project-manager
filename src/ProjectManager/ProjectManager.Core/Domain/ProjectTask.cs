using ProjectManager.Core.SeedWork.Domain;
using System;
using System.Collections.Generic;

namespace ProjectManager.Core.Domain
{
    public sealed class ProjectTask : BaseEntity, IAggregateRoot
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}