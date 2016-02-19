namespace Euventing.Atom.Document
{
    public class AtomDocumentFullEvent
    {
        public AtomDocumentFullEvent(DocumentId documentId)
        {
            DocumentId = documentId;
        }

        public DocumentId DocumentId { get; private set; }
    }
}