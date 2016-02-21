namespace Euventing.Atom.Document
{
    internal class EventAddedToDocument
    {
        public int CurrentEvents { get; }

        public EventAddedToDocument(int currentEvents)
        {
            CurrentEvents = currentEvents;
        }
    }
}