namespace Euventing.Atom.Document
{
    public class AtomFeedDocumentHeadChanged
    {
        public DocumentId CurrentHeadDocumentId { get; }
        public DocumentId EarlierDocumentId { get; }

        public AtomFeedDocumentHeadChanged(DocumentId headDocument, DocumentId earlierDocumentId)
        {
            this.CurrentHeadDocumentId = headDocument;
            this.EarlierDocumentId = earlierDocumentId;
        }
    }
}