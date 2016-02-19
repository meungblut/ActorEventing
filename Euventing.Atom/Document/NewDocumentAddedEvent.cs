namespace Euventing.Atom.Document
{
    public class NewDocumentAddedEvent
    {
        public DocumentId DocumentId { get; }

        public NewDocumentAddedEvent(DocumentId documentId)
        {
            this.DocumentId = documentId;
        }
    }
}