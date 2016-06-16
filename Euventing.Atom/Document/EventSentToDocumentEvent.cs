namespace Eventing.Atom.Document
{
    internal class EventSentToDocumentEvent
    {
        public int CurrentEventsProcessed { get; }
        private DocumentId currentFeedHeadDocument;

        public EventSentToDocumentEvent(DocumentId currentFeedHeadDocument, int currentEventsProcessed)
        {
            this.currentFeedHeadDocument = currentFeedHeadDocument;
            this.CurrentEventsProcessed = currentEventsProcessed;
        }
    }
}