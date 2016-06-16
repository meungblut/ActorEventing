namespace Eventing.Core.Messages
{
    public class SubscriptionId
    {
        public SubscriptionId(string uuid)
        {
            Id = uuid;
        }

        public string Id { get; }


    }
}
