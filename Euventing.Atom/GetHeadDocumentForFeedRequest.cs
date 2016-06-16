using Eventing.Core.Messages;

namespace Eventing.Atom
{
    public class GetHeadDocumentForFeedRequest
    {
        public SubscriptionId SubscriptionId { get; }

        public GetHeadDocumentForFeedRequest(SubscriptionId subscriptionId)
        {
            this.SubscriptionId = subscriptionId;
        }
    }
}