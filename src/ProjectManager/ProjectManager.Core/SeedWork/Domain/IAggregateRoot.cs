using System;

namespace ProjectManager.Core.SeedWork.Domain
{
    public interface IAggregateRoot
    {
        DateTime CreatedDate { get; set; }
        DateTime? ModifiedDate { get; set; }
    }
}