using Euventing.Atom.Document;

namespace Euventing.Atom.Test
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