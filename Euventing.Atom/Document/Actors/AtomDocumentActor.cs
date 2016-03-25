using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using Akka.Persistence;
using Euventing.Atom.Serialization;
using Euventing.Core.Messages;

namespace Euventing.Atom.Document.Actors
{
    public class AtomDocumentActor : AtomDocumentActorBase
    {
        private DateTime startMarker;
        private DateTime serialisedMarker;
        private DateTime persistedMarker;

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

        private void Process(CreateAtomDocumentCommand creationRequest)
        {
            LoggingAdapter.Debug("Creating document " + creationRequest.DocumentId.Id + " on node " + Cluster.Get(Context.System).SelfAddress);
            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.EarlierEventsDocumentId);

            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, MutateInternalState);
        }

        protected void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.Author = documentCreated.Author;
            this.DocumentId = documentCreated.DocumentId;
            this.EarlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.Title = documentCreated.Title;
            this.FeedId = documentCreated.FeedId;
        }

        private void Process(NewDocumentAddedEvent newDocumentEvent)
        {
            MutateInternalState(newDocumentEvent);
            Persist(newDocumentEvent, null);
            Context.Parent.Tell(new Passivate(PoisonPill.Instance));
        }

        private void Process(ReceiveTimeout timeoutEvent)
        {
            Context.Parent.Tell(new Passivate(PoisonPill.Instance));
        }

        private void Process(Stop stopMessage)
        {
            Context.Stop(Self);
        }

        private void MutateInternalState(NewDocumentAddedEvent newDocumentEvent)
        {
            this.LaterEventsDocumentId = newDocumentEvent.DocumentId;
        }

        private void Process(EventWithDocumentIdNotificationMessage eventToAdd)
        {
            startMarker = DateTime.Now;
            var converter = new DomainEventToAtomEntryConverter();
            var atomEntry = converter.ConvertDomainEventToAtomEntry(eventToAdd.DomainEvent);
            serialisedMarker = DateTime.Now;
            Persist(atomEntry, MutateInternalState);
        }

        private void Process(GetAtomDocumentRequest request)
        {
            LoggingAdapter.Debug("Request for document id {0} on node {2} with events {3}",
     PersistenceId, Cluster.Get(Context.System).SelfAddress, Entries.Count);
            
            GetCurrentAtomDocument();
        }

        private void Process(object request)
        {
            LoggingAdapter.Info("Unhandled command " + request.GetType());
        }

        private void MutateInternalState(RecoveryCompleted documentCreated)
        {
        }

        private void MutateInternalState(AtomEntry atomEntry)
        {
            Entries.Add(atomEntry);
            SequenceNumber = SequenceNumber + 1;
            Updated = atomEntry.Updated;

            persistedMarker = DateTime.Now;

            LoggingAdapter.Debug("started {0:h:mm:ss tt ffff}, serialised {1:h:mm:ss tt ffff}, persisted {2:h:mm:ss tt ffff}",
                startMarker, serialisedMarker, persistedMarker);

            LoggingAdapter.Debug("Added event with id {0} to doc {1} in feed {2} on node {3}",
    SequenceNumber, this.DocumentId.Id, this.FeedId.Id, Cluster.Get(Context.System).SelfAddress);
        }
    }
}
