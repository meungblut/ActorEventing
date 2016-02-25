using System;
using System.Runtime.Serialization;

namespace Euventing.Core
{
    [Serializable]
    public class CouldNotProcessPersistenceMessage : Exception
    {
        public CouldNotProcessPersistenceMessage()
        {
        }

        public CouldNotProcessPersistenceMessage(string message) : base(message)
        {
        }

        public CouldNotProcessPersistenceMessage(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotProcessPersistenceMessage(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}