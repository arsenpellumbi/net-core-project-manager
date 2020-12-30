using System;
using System.Runtime.Serialization;

namespace ProjectManager.Core.SeedWork.Domain
{
    [Serializable]
    public class UnAuthenticatedException : Exception
    {
        public UnAuthenticatedException(string message)
            : base(message)
        {
        }

        public UnAuthenticatedException()
        {
        }

        public UnAuthenticatedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UnAuthenticatedException(
            SerializationInfo serializationInfo, StreamingContext streamingContext
        ) : base(serializationInfo, streamingContext)
        {
        }
    }
}