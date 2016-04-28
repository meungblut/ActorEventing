using System;
using Euventing.Atom.Document;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
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
