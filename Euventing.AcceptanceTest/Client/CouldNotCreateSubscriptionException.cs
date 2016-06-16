using System;
using System.Runtime.Serialization;

namespace Eventing.AcceptanceTest.Client
{
    [Serializable]
    internal class CouldNotCreateSubscriptionException : Exception
    {
        public CouldNotCreateSubscriptionException()
        {
        }

        public CouldNotCreateSubscriptionException(string message) : base(message)
        {
        }

        public CouldNotCreateSubscriptionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouldNotCreateSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}