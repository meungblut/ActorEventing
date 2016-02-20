using Euventing.Core.Messages;

namespace Euventing.Atom
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