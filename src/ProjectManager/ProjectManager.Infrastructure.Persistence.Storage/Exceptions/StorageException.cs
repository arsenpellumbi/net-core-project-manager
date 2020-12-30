using System;
using System.Runtime.Serialization;

namespace AI.OrchestrationEngine.Infrastructure.Persistence.Storage.Exceptions
{
    /// <summary>
    /// Exception type for storage exceptions
    /// </summary>
    [Serializable]
    public class StorageException : Exception
    {
        public StorageException()
        { }

        public StorageException(string message)
            : base(message)
        { }

        public StorageException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected StorageException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }
}