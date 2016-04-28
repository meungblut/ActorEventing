using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Feed;
using Euventing.Atom.Document;
using Euventing.Core;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Subscription
{
    public class SubscriptionActor : PersistentActorBase
    {
        private SubscriptionMessage subscriptionMessage;
        private readonly Cluster cluster;
        private readonly List<IActorRef> documentActors = new List<IActorRef>();
        private readonly IAtomDocumentSettings atomDocumentSettings;

        private int CurrentHeadDocumentIndex = 0;

        public SubscriptionActor(IAtomDocumentSettings settings)
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
            LogTraceInfo("Received delete subscription message");
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadDocumentForFeedRequest)
        {
            LogTraceInfo("Getting atom document in subscription actor");

            Sender.Tell(new DocumentId(CurrentHeadDocumentIndex.ToString()));
        }

        private void Process(SubscriptionMessage subscription)
        {
            Persist(subscription, MutateInternalState);
        }

        private void CreateFeedActor(SubscriptionMessage subscription)
        {
            LogTraceInfo($"Creating atom subscription actor");
            CreateDocumentOnEachNode(subscription);
            LogTraceInfo("Created atom subscription actor");
        }

        private void CreateDocumentOnEachNode(SubscriptionMessage subscription)
        {
            foreach (var member in cluster.ReadView.Members)
            {
                var props =
                    Props.Create<AtomDocumentActor>(
                        () =>
                            new AtomDocumentActor(new AtomDocumentSettings(),
                                new InMemoryAtomDocumentRepository()));

                var atomDocument =
                     Context.System.ActorOf(
                         props
                         .WithDeploy(
                             new Deploy(
                                 new RemoteScope(member.Address))), "atomActor_" + member.Address.GetHashCode() + "_" + subscriptionMessage.SubscriptionId.Id);

                atomDocument.Tell(
                    new CreateAtomDocumentCommand(
                        "", "", new FeedId(subscription.SubscriptionId.Id)));

                documentActors.Add(atomDocument);

                LogTraceInfo($"Subscription Actor deployed with address {atomDocument.Path} ");
            }
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
            LogTraceInfo($"Unhandled command {unhandledObject.GetType()} {unhandledObject} from {Context.Sender.Path}");
        }

        private void MutateInternalState(SubscriptionMessage subscription)
        {
            this.subscriptionMessage = subscription;

            CreateFeedActor(subscription);
        }

        private void MutateInternalState(RecoveryCompleted recoveryCompleted)
        {
            UnstashAll();
        }

        private void MutateInternalState(object unhandledObject)
        {
            LogTraceInfo($"Unhandled persistence command");
        }
    }
}
