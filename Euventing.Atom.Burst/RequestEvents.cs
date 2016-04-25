namespace Euventing.Atom.Burst
{
    public class RequestEvents
    {
        public int EventsToSend { get; }
        public long LastProcessedId { get; }

        public RequestEvents(int eventsToSend, long lastProcessedId)
        {
            EventsToSend = eventsToSend;
            LastProcessedId = lastProcessedId;
        }
    }
}
