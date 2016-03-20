using Euventing.Atom.Document;

namespace Euventing.Atom.Burst
{
    public class QueuedEvent
    {
        public AtomEntry Message { get; }
        public int QueueLength { get; }

        public QueuedEvent(AtomEntry message, int queueLength)
        {
            Message = message;
            QueueLength = queueLength;
        }
    }
}