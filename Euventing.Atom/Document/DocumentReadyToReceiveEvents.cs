namespace Eventing.Atom.Document
{
    public class DocumentReadyToReceiveEvents
    {
        public DocumentId DocumentId { get; }

        public DocumentReadyToReceiveEvents(DocumentId documentId)
        {
            this.DocumentId = documentId;
        }
    }
}