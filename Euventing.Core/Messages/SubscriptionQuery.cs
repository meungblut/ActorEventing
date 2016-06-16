namespace Eventing.Core.Messages
{
    public class SubscriptionQuery
    {
        public SubscriptionId SubscriptionId { get; private set; }


        public SubscriptionQuery(SubscriptionId subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}
