namespace Eventing.Core.Messages
{
    public class DeleteSubscriptionMessage
    {
        public SubscriptionId SubscriptionId { get; set; }

        public DeleteSubscriptionMessage(SubscriptionId subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}