using Akka.Actor;
using Akka.Cluster;
using Euventing.Atom.Burst.Feed;
using Euventing.Atom.Document;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Subscription
{
    public class BurstSubscriptionActor : PersistentActorBase
    {
        private SubscriptionMessage subscriptionMessage;
        private readonly Cluster cluster;
        private readonly ShardedAtomFeedFactory shardedAtomFeedFactory;

        public BurstSubscriptionActor(ShardedAtomFeedFactory atomFeedFactory)
        {
            shardedAtomFeedFactory = atomFeedFactory;
            cluster = Cluster.Get(Context.System);
        }

        protected override bool ReceiveRecover(object message)
        {
            if (message == null)
                return false;

            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
                return false;

            ((dynamic)this).Process((dynamic)message);

            return true;
        }


        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            LoggingAdapter.Debug("Received delete subscription message " + subscriptionMessage.SubscriptionId.Id);
        }

        private void Process(SubscriptionMessage subscription)
        {
            Persist(subscription, MutateInternalState);

            CreateSubscriptionOnEachNode(subscription);

            CreateFeedActor(subscription);
        }

        private void CreateFeedActor(SubscriptionMessage subscription)
        {
            var actor = shardedAtomFeedFactory.GetActorRef();
            actor.Tell(new AtomFeedCreationCommand("", "", new FeedId(subscription.SubscriptionId.Id), null));
        }


        private void Process(SubscriptionQuery query)
        {
            if (subscriptionMessage == null)
                Sender.Tell(new NullSubscription(), Context.Self);
            else
                Sender.Tell(subscriptionMessage, Context.Self);
        }

        private void Process(object unhandledObject)
        {
            LoggingAdapter.Debug("Subscription Actor. Unhandled command " + unhandledObject.GetType());
        }

        private void MutateInternalState(SubscriptionMessage message)
        {
            this.subscriptionMessage = message;
        }

        private void MutateInternalState(object unhandledObject)
        {
            LoggingAdapter.Debug("Subscription Actor. Unhandled persistence command " + unhandledObject.GetType());
        }

        private void CreateSubscriptionOnEachNode(SubscriptionMessage message)
        {
            foreach (var member in cluster.ReadView.Members)
            {
                var subscriptionActor =
                     Context.System.ActorOf(
                         Props.Create<SubscriptionQueueActor>()
                         .WithDeploy(
                             new Deploy(
                                 new RemoteScope(member.Address))), "subscription_" + message.SubscriptionId.Id);
            }
        }
    }
}
