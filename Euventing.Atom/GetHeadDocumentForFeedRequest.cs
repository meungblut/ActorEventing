using Euventing.Core.Messages;

namespace Euventing.Atom
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