using Euventing.Atom.Document;

namespace Euventing.Atom.Burst
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