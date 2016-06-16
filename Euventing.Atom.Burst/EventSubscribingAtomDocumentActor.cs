﻿using System;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class EventSubscribingAtomDocumentActor : AtomDocumentActorBase
    {
        protected Cluster Cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;
        private readonly IAtomDocumentRepository repository;
        private DocumentId CurrentDocumentId;
        private readonly DomainEventToAtomEntryConverter converter = new DomainEventToAtomEntryConverter();

        private DateTime DocumentCreationDate;

        private IntervalBasedDate dateInterval;
        private TimeSpan timeBeforeMovingToNextDocumentId = TimeSpan.FromSeconds(10);

        public EventSubscribingAtomDocumentActor(IAtomDocumentSettings settings, IAtomDocumentRepository repository)
        {
            this.repository = repository;
            atomDocumentSettings = settings;
            Context.System.EventStream.Subscribe(Context.Self, typeof(DomainEvent));
        }

        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            LogTraceInfo($"Cancelling document");
            Context.System.EventStream.Unsubscribe(Context.Self, typeof(DomainEvent));
        }

        private void Process(DomainEvent domainEvent)
        {
            var documentId = new DocumentId(CurrentDocumentId.FeedId, dateInterval.Current.ToFileTime());
            repository.Add(this.CurrentDocumentId, converter.ConvertDomainEventToAtomEntry(domainEvent));
        }

        private void Process(CreateAtomDocumentCommand createDocument)
        {
            var createdEvent = new AtomDocumentCreatedEvent(
                createDocument.Title,
                createDocument.Author,
                createDocument.DocumentId);

            Persist(createdEvent, MutateInternalState);
        }

        protected void MutateInternalState(AtomDocumentCreatedEvent documentCreated)
        {
            this.Author = documentCreated.Author;
            this.DocumentId = documentCreated.DocumentId;
            this.CurrentDocumentId = documentCreated.DocumentId;
            this.EarlierEventsDocumentId = documentCreated.EarlierEventsDocumentId;
            this.Title = documentCreated.Title;
            this.FeedId = documentCreated.DocumentId.FeedId;

            DocumentCreationDate = DateTime.Now;
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
}