namespace Euventing.Atom.Burst
{
    public class RequestEvents
    {
        public int MaxEventsToSend { get; }
    
        public RequestEvents(int maxEventsToSend)
        {
            MaxEventsToSend = maxEventsToSend;
        }
    }
}
