﻿using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using CassandraRepository;
using Eventing.Atom.Burst.Feed;
using Eventing.Atom.Document;
using Eventing.Core;
using Eventing.Core.Messages;

namespace Eventing.Atom.Burst.Subscription
{
    public class SubscriptionActor : PersistentActorBase
    {
        private SubscriptionMessage subscriptionMessage;
        private readonly Cluster cluster;
        private readonly List<IActorRef> documentActors = new List<IActorRef>();
        private readonly IAtomDocumentSettings atomDocumentSettings;

        private readonly IAtomDocumentRepository atomRepository;

        private DocumentId headDocumentIdForFeed;

        public SubscriptionActor(IAtomDocumentSettings settings)
        {
            //atomRepository = new InMemoryAtomDocumentRepository();
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
            foreach (var documentActor in documentActors)
            {
                documentActor.Tell(deleteSubscription);
            }
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadDocumentForFeedRequest)
        {
            LogTraceInfo("Getting atom document in subscription actor");

            Sender.Tell(headDocumentIdForFeed);
        }

        private void Process(NewDocumentAddedEvent newDocumentAddedEvent)
        {
            Persist(newDocumentAddedEvent, MutateInternalState);
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
                    Props.Create(
                        () =>
                            new EventSubscribingAtomDocumentActor(new AtomDocumentSettings()));

                var nameOfActorToDeployOnNode = $"atomActor_{member.Address.GetHashCode()}_{subscriptionMessage.SubscriptionId.Id}";

                LogTraceInfo($"About to deploy actor with name {nameOfActorToDeployOnNode} onto node {member.Address}");

                var atomDocument =
                     Context.ActorOf(
                         props
                         .WithDeploy(
                             new Deploy(
                                 new RemoteScope(member.Address))), nameOfActorToDeployOnNode);

                atomDocument.Tell(
                    new CreateAtomDocumentCommand(
                        "", "", headDocumentIdForFeed, Context.Self));

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

        private void Process(DocumentMovedToNewId moved)
        {
            headDocumentIdForFeed = moved.DocumentId;
        }

        private void Process(object unhandledObject)
        {
            LogTraceInfo($"Unhandled command {unhandledObject.GetType()} {unhandledObject} from {Context.Sender.Path}");
        }

        private void MutateInternalState(SubscriptionMessage subscription)
        {
            this.subscriptionMessage = subscription;

            headDocumentIdForFeed = new DocumentId(subscription.SubscriptionId.Id, 0);

            CreateFeedActor(subscription);
        }

        private void MutateInternalState(NewDocumentAddedEvent newDocumentAddedEvent)
        {
            this.headDocumentIdForFeed = newDocumentAddedEvent.DocumentId;
        }

        private void MutateInternalState(RecoveryCompleted recoveryCompleted)
        {
            //UnstashAll();
        }

        private void MutateInternalState(object unhandledObject)
        {
            LogTraceInfo($"Unhandled persistence command");
        }
    }
}
