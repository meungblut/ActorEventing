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

        private int recoveryevents;

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
            loggingAdapter.Debug("Creating document " + creationRequest.DocumentId.Id + " on node " + Cluster.Get(Context.System).SelfAddress);
            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.EarlierEventsDocumentId);

            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, MutateInternalState);
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
            this.laterEventsDocumentId = newDocumentEvent.DocumentId;
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
            loggingAdapter.Debug("Request for document id {0} on node {2} with events {3}",
     PersistenceId, Cluster.Get(Context.System).SelfAddress, entries.Count);
            
            GetCurrentAtomDocument();
        }

        private void Process(object request)
        {
            loggingAdapter.Info("Unhandled command " + request.GetType());
        }

        private void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.author = documentCreated.Author;
            this.documentId = documentCreated.DocumentId;
            this.earlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.title = documentCreated.Title;
            this.feedId = documentCreated.FeedId;
            loggingAdapter.Debug("Setting Feed Id to " + feedId.Id + " and document id to " + documentId.Id + " on persistence id " + PersistenceId);
        }

        private void MutateInternalState(RecoveryCompleted documentCreated)
        {
        }

        private void MutateInternalState(AtomEntry atomEntry)
        {
            entries.Add(atomEntry);
            sequenceNumber = sequenceNumber + 1;
            updated = atomEntry.Updated;

            persistedMarker = DateTime.Now;

            loggingAdapter.Debug("started {0:h:mm:ss tt ffff}, serialised {1:h:mm:ss tt ffff}, persisted {2:h:mm:ss tt ffff}",
                startMarker, serialisedMarker, persistedMarker);

            loggingAdapter.Debug("Added event with id {0} to doc {1} in feed {2} on node {3}",
    sequenceNumber, this.documentId.Id, this.feedId.Id, Cluster.Get(Context.System).SelfAddress);
        }
    }
}
