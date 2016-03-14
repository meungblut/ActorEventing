namespace Euventing.Core.Messages
{
    public class DeleteSubscriptionMessage
    {
        public SubscriptionId SubscriptionId { get; private set; }

        public DeleteSubscriptionMessage(SubscriptionId subscriptionId)
        {
            SubscriptionId = subscriptionId;
        }
    }
}