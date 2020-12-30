using System;
using System.Runtime.Serialization;

namespace ProjectManager.Core.SeedWork.Domain
{
    /// <summary>
    /// Exception type for domain exceptions
    /// </summary>
    [Serializable]
    public class ResourceInUseException : Exception
    {
        public ResourceInUseException(string message) : base(message)
        {
        }

        public ResourceInUseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResourceInUseException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public ResourceInUseException()
        {
        }
    }
}