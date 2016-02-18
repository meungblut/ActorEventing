namespace Euventing.Atom.Document
{
    public class GetAtomDocumentRequest
    {
        public DocumentId DocumentId { get; }

        public GetAtomDocumentRequest(DocumentId documentId)
        {
            this.DocumentId = documentId;
        }
    }
}