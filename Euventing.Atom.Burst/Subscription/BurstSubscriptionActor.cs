using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
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
        private readonly List<IActorRef> subscriptionQueues = new List<IActorRef>();
        private readonly IAtomDocumentSettings atomDocumentSettings;
        private IActorRef atomFeedActor;

        public BurstSubscriptionActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
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
            atomFeedActor.Forward(deleteSubscription);
            LogInfo("Received delete subscription message");
        }

        private void Process(GetHeadDocumentForFeedRequest getHeadDocumentForFeedRequest)
        {
            LogInfo("Getting atom document in subscription actor");

            if (atomFeedActor == null)
            {
                LogInfo($"Feed actor was null");
                throw new NullReferenceException();
            }

            atomFeedActor.Forward(getHeadDocumentForFeedRequest);
        }

        private void Process(SubscriptionMessage subscription)
        {
            Persist(subscription, MutateInternalState);
        }

        private void CreateFeedActor(SubscriptionMessage subscription)
        {
            LogInfo($"Creating atom feed actor");
            var props = Props.Create(() => new FeedActor(this.atomDocumentSettings));
            atomFeedActor = Context.ActorOf(props, "Feed_" + subscription.SubscriptionId.Id);
            atomFeedActor.Tell(new SubscriptionsAtomFeedShouldPoll(subscriptionQueues));
            atomFeedActor.Tell(new AtomFeedCreationCommand("", "", new FeedId(subscription.SubscriptionId.Id), null));
            LogInfo("Created atom feed actor");

        }

        private void Process(SubscriptionQuery query)
        {
            if (subscriptionMessage == null)
                Sender.Tell(new NullSubscription(), Context.Self);
            else
                Sender.Tell(subscriptionMessage, Context.Self);
        }

        private void Process(GetDocumentFromFeedRequest getDocumentRequest)
        {
            LogInfo($"Asking for document with id {getDocumentRequest.DocumentId.Id}");

            atomFeedActor.Forward(getDocumentRequest);
        }

        private void Process(object unhandledObject)
        {
            LogInfo($"Unhandled command {unhandledObject.GetType()} {unhandledObject} from {Context.Sender.Path}");
        }

        private void MutateInternalState(SubscriptionMessage subscription)
        {
            this.subscriptionMessage = subscription;

            CreateSubscriptionOnEachNode(subscription);

            CreateFeedActor(subscription);
        }

        private void MutateInternalState(object unhandledObject)
        {
            LogInfo($"Unhandled persistence command");
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
                                 new RemoteScope(member.Address))), "subscription_" + member.Address.GetHashCode() + "_" + message.SubscriptionId.Id);

                subscriptionQueues.Add(subscriptionActor);

                LogInfo($"Subscription Actor deployed with address {subscriptionActor.Path} ");
            }
        }
    }
}
