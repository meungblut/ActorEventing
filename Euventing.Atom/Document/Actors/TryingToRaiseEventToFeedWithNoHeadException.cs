using System;
using System.Runtime.Serialization;

namespace Euventing.Atom.Document.Actors
{
    [Serializable]
    internal class TryingToRaiseEventToFeedWithNoHeadException : Exception
    {
        public TryingToRaiseEventToFeedWithNoHeadException()
        {
        }

        public TryingToRaiseEventToFeedWithNoHeadException(string message) : base(message)
        {
        }

        public TryingToRaiseEventToFeedWithNoHeadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TryingToRaiseEventToFeedWithNoHeadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}