using System.Threading.Tasks;
using Euventing.Core.Messages;

namespace Euventing.Core.Subscriptions
{
    public interface ISubscriptionManager
    {
        void CreateSubscription(SubscriptionMessage subscriptionMessage);
        void DeleteSubscription(DeleteSubscriptionMessage subscriptionMessage);
        Task<SubscriptionMessage> GetSubscriptionDetails(SubscriptionQuery query);
    }
}