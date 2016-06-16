using System.Threading.Tasks;
using Eventing.Core.Messages;

namespace Eventing.Core.Subscriptions
{
    public interface ISubscriptionManager
    {
        void CreateSubscription(SubscriptionMessage subscriptionMessage);
        void DeleteSubscription(DeleteSubscriptionMessage subscriptionMessage);
        Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query);
    }
}