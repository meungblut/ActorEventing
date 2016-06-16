using Eventing.Atom.Document;

namespace Eventing.Atom.Burst
{
    public class QueuedEvent
    {
        public AtomEntry Message { get; }

        public QueuedEvent(AtomEntry message)
        {
            Message = message;
        }
    }
}