using System;

namespace ProjectManager.Core.SeedWork.Domain
{
    public interface IIdGenerator
    {
        Guid NewId();
    }
}