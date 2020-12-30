using ProjectManager.Core.SeedWork.Domain;
using System;

namespace ProjectManager.API.Infrastructure
{
    /// <summary>
    /// Id generator
    /// </summary>
    public class IdGenerator : IIdGenerator
    {
        /// <summary>
        /// Generate new id
        /// </summary>
        /// <returns></returns>
        public Guid NewId()
        {
            return MassTransit.NewId.NextGuid();
        }
    }
}