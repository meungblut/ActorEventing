using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class WorkPullingDocumentActor : AtomDocumentActorBase, IWithUnboundedStash
    {
        protected Cluster Cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;

        private int entriesInCurrentDocument;

        public WorkPullingDocumentActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
        }

        private void PollQueues(FeedId id)
        {
            Cluster = Cluster.Get(Context.System);

            string addressFormat = "{0}/user/subscription_" + id.Id;

            foreach (var member in Cluster.ReadView.Members)
            {
                var address = string.Format(addressFormat, member.Address);
                var actorRef = Context.System.ActorSelection(address);
                actorRef.Tell(new RequestEvents(50));
            }
        }

        protected void Process(CreateAtomDocumentCommand creationRequest)
        {
            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.EarlierEventsDocumentId);

            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, MutateInternalState);

            PollQueues(creationRequest.FeedId);
        }

        private void Process(List<QueuedEvent> requestedEvents)
        {
            int outstandingEvents = 0;
            foreach (var requestedEvent in requestedEvents)
            {
                Persist(requestedEvent.Message, MutateInternalState);
                outstandingEvents = requestedEvent.QueueLength;
            }

            if (outstandingEvents > 0)
                LoggingAdapter.Info($"{outstandingEvents} remain in queue");

            if (entriesInCurrentDocument >= atomDocumentSettings.NumberOfEventsPerDocument)
            {
                DocumentIsFull();
            }
            else
            {
                Self.Tell(new PollForEvents());
            }
        }

        private void DocumentIsFull()
        {
            var documentId = new DocumentId((int.Parse(DocumentId.Id) + 1).ToString());
            var addressToDeployOn = GetDifferentNodeIfPossible();

            var props = Props.Create(() => new WorkPullingDocumentActor(atomDocumentSettings));

            var newActor =
                Context.System.ActorOf(
                    props.
                    WithDeploy(new Deploy(new RemoteScope(addressToDeployOn))));
            newActor.Tell(new CreateAtomDocumentCommand(this.Title, this.Author, this.FeedId, documentId, this.DocumentId));
        }

        private Address GetDifferentNodeIfPossible()
        {
            if (Cluster.ReadView.IsSingletonCluster)
                return Cluster.ReadView.SelfAddress;

            return Cluster.ReadView.Members.First(x => x.Address != Cluster.SelfAddress).Address;
        }

        private void MutateInternalState(AtomEntry entry)
        {
            Entries.Add(entry);
            LoggingAdapter.Info($"Added event {entry.Id} to feed {FeedId.Id} document {DocumentId.Id}");
            entriesInCurrentDocument++;
        }

        private void Process(GetAtomDocumentRequest request)
        {
            GetCurrentAtomDocument();
        }

        private void Process(PollForEvents request)
        {
            PollQueues(FeedId);
        }

        protected void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.Author = documentCreated.Author;
            this.DocumentId = documentCreated.DocumentId;
            this.EarlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.Title = documentCreated.Title;
            this.FeedId = documentCreated.FeedId;
        }

        private void MutateInternalState(RecoveryCompleted complete)
        {
            this.UnstashAll();
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
                return false;

            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        protected override bool ReceiveRecover(object message)
        {
            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }
    }

    public class PollForEvents
    {
    }
}
