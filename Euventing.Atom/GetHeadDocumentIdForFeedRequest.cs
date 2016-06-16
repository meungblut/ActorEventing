using Eventing.Core.Messages;

namespace Eventing.Atom
{
    public class GetHeadDocumentIdForFeedRequest
    {
        public SubscriptionId SubscriptionId { get; }

        public GetHeadDocumentIdForFeedRequest(SubscriptionId subscriptionId)
        {
            this.SubscriptionId = subscriptionId;
        }
    }
}