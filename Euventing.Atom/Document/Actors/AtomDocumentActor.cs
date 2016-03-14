﻿using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Akka.Cluster.Sharding;
using Akka.Dispatch.SysMsg;
using Akka.Event;
using Akka.Persistence;
using Euventing.Atom.Serialization;

namespace Euventing.Atom.Document.Actors
{
    public class AtomDocumentActor : PersistentActor
    {

        public AtomDocumentActor(IAtomDocumentSettings settings)
        {
            loggingAdapter = Context.GetLogger();
            loggingAdapter.Info("Atom DOCUMENT actor path is " + Self.Path);
            PersistenceId = "AtomDocumentActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        private string title;
        private DateTime updated;
        private string author;
        private FeedId feedId;
        private DocumentId documentId;
        private DocumentId laterEventsDocumentId;
        private DocumentId earlierEventsDocumentId;
        private readonly List<AtomEntry> entries = new List<AtomEntry>();

        private long sequenceNumber;
        private readonly ILoggingAdapter loggingAdapter;

        private DateTime startMarker;
        private DateTime serialisedMarker;
        private DateTime persistedMarker;

        protected override void PreStart()
        {
            base.PreStart();

            ////Take this instance out of memory after inactivity
            //Context.SetReceiveTimeout(TimeSpan.FromSeconds(10));
        }

        private int recoveryevents;

        protected override bool ReceiveRecover(object message)
        {
            loggingAdapter.Info("AtomDocumentActor ReceiveRecover: " + message.GetType() + " with persistence id:" + PersistenceId + " times called :" + ++recoveryevents);

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

        public override string PersistenceId { get; }

        private void Process(CreateAtomDocumentCommand creationRequest)
        {
            loggingAdapter.Info("Creating document " + creationRequest.DocumentId.Id + " on node " + Cluster.Get(Context.System).SelfAddress);
            var atomDocumentCreatedEvent = new AtomDocumentCreatedEvent(creationRequest.Title,
                creationRequest.Author, creationRequest.FeedId, creationRequest.DocumentId, creationRequest.EarlierEventsDocumentId);

            MutateInternalState(atomDocumentCreatedEvent);
            Persist(atomDocumentCreatedEvent, MutateInternalState);
        }

        private void Process(NewDocumentAddedEvent newDocumentEvent)
        {
            MutateInternalState(newDocumentEvent);
            Persist(newDocumentEvent, null);
            loggingAdapter.Info("Passivating " + this.PersistenceId);
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
            var atomEntry = new AtomEntry();
            var serializer = new JsonEventSerialisation();
            var content = serializer.GetContentWithContentType(eventToAdd.DomainEvent);
            atomEntry.Content = content.Content;
            atomEntry.Id = eventToAdd.DomainEvent.Id;
            atomEntry.Updated = DateTime.Now;
            atomEntry.Title = content.ContentType;
            sequenceNumber = sequenceNumber + 1;
            atomEntry.SequenceNumber = sequenceNumber;
            serialisedMarker = DateTime.Now;
            Persist(atomEntry, MutateInternalState);
        }

        private void Process(GetAtomDocumentRequest request)
        {
            loggingAdapter.Info("Request for document id {0} on node {2} with events {3}",
     PersistenceId, Cluster.Get(Context.System).SelfAddress, entries.Count);


            var atomDocument = new AtomDocument(title, author, feedId, documentId, earlierEventsDocumentId,
                laterEventsDocumentId, entries);
            atomDocument.AddDocumentInformation(Cluster.Get(Context.System).SelfAddress.ToString());
            Sender.Tell(atomDocument, Self);
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
            loggingAdapter.Info("Setting Feed Id to " + feedId.Id + " and document id to " + documentId.Id +" on persistence id " + PersistenceId);
        }

        private void MutateInternalState(RecoveryCompleted documentCreated)
        {
        }

        private void MutateInternalState(AtomEntry atomEntry)
        {
            entries.Add(atomEntry);
            sequenceNumber = atomEntry.SequenceNumber;
            updated = atomEntry.Updated;

            persistedMarker = DateTime.Now;

            loggingAdapter.Info("started {0:h:mm:ss tt ffff}, serialised {1:h:mm:ss tt ffff}, persisted {2:h:mm:ss tt ffff}",
                startMarker, serialisedMarker, persistedMarker);

            loggingAdapter.Info("Added event with id {0} to doc {1} in feed {2} on node {3}",
    atomEntry.SequenceNumber, this.documentId.Id, this.feedId.Id, Cluster.Get(Context.System).SelfAddress);
        }
    }
}
