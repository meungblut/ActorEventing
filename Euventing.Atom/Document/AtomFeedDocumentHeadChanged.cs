namespace Euventing.Atom.Document
{
    public class AtomFeedDocumentHeadChanged
    {
        public DocumentId DocumentId { get; set; }

        public AtomFeedDocumentHeadChanged(DocumentId documentId)
        {
            this.DocumentId = documentId;
        }
    }
}