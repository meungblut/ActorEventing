using Euventing.Atom.Document;

namespace Euventing.Atom.Burst
{
    public class RequestEvents
    {
        public int EventsToSend { get; }
        public long LastProcessedId { get; }
        public FeedId FeedId { get; }

        public RequestEvents(int eventsToSend, long lastProcessedId, FeedId feedId)
        {
            EventsToSend = eventsToSend;
            LastProcessedId = lastProcessedId;
            FeedId = feedId;
        }
    }
}
