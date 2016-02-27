namespace Euventing.Atom.Document
{
    public class AtomFeedDocumentHeadChanged
    {
        public DocumentId CurrentHeadDocumentId { get; }
        public int CurrentDocumentIndex { get; }
        public DocumentId EarlierDocumentId { get; }

        public AtomFeedDocumentHeadChanged(DocumentId headDocument, DocumentId earlierDocumentId, int currentDocumentIndex)
        {
            this.CurrentHeadDocumentId = headDocument;
            this.EarlierDocumentId = earlierDocumentId;
            this.CurrentDocumentIndex = currentDocumentIndex;
        }
    }
}