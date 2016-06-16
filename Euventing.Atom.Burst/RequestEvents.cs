using System;
using Eventing.Atom.Document;

namespace Eventing.Atom.Burst
{
    public class RequestEvents
    {
        public int EventsToSend { get; }
        public DateTime EarliestEventsToSend { get; set; }
        public long LastProcessedId { get; }
        public FeedId FeedId { get; }

        public RequestEvents(int eventsToSend, long lastProcessedId, FeedId feedId, DateTime earliestEventsToSend)
        {
            EventsToSend = eventsToSend;
            LastProcessedId = lastProcessedId;
            FeedId = feedId;
            EarliestEventsToSend = earliestEventsToSend;
        }
    }
}
