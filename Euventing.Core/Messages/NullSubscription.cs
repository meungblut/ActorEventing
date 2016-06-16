namespace Eventing.Core.Messages
{
    public class NullSubscription : SubscriptionMessage
    {
        public NullSubscription() : base(null, null, null)
        {
        }
    }
}
