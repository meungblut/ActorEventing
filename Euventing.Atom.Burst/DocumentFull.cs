using Euventing.Atom.Document;

namespace Euventing.Atom.Burst
{
    internal class DocumentFull
    {
        public FeedId FeedId { get; private set; }
        public DocumentId DocumentId { get; private set; }

        public DocumentFull(FeedId feedId, DocumentId documentId)
        {
            FeedId = feedId;
            DocumentId = documentId;
        }
    }
}