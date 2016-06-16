using Eventing.Atom.Document;
using Eventing.Core.Messages;

namespace Eventing.Atom.Burst
{
    public class GetDocumentFromFeedRequest
    {
        public SubscriptionId SubscriptionId { get; }
        public DocumentId DocumentId { get; }

        public GetDocumentFromFeedRequest(SubscriptionId subscriptionId, DocumentId documentId)
        {
            SubscriptionId = subscriptionId;
            DocumentId = documentId;
        }
    }
}
