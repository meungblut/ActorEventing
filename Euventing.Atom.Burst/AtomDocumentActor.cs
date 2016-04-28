﻿using System;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst
{
    public class AtomDocumentActor : AtomDocumentActorBase, IWithUnboundedStash
    {
        protected Cluster Cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;
        private int entriesInCurrentDocument;

        private bool feedCancelled = false;

        private int currentDocumentIndex;

        private long lastEventIdProcessed;
        private readonly IAtomDocumentRepository _repository;

        private IActorRef eventQueueOnThisNode;

        public AtomDocumentActor(IAtomDocumentSettings settings, IAtomDocumentRepository repository)
        {
            _repository = repository;
            atomDocumentSettings = settings;

            eventQueueOnThisNode = ActorLocations.LocalQueueActor;

            PollSubscriptionQueue(eventQueueOnThisNode);
        }

        private void PollSubscriptionQueue(IActorRef actorRef)
        {
            LogTraceInfo($"Asking for events from node {actorRef.Path}");

            var eventsToRequest = atomDocumentSettings.NumberOfEventsPerDocument - entriesInCurrentDocument;

            actorRef.Tell(new RequestEvents(eventsToRequest, lastEventIdProcessed));
        }

        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            LogTraceInfo($"Cancelling document");

            feedCancelled = true;
        }

        private void Process(RequestedEvents<AtomEntry> requestedEvents)
        {
            AddEventsToDocument(requestedEvents);

            if (feedCancelled)
                return;

            Thread.Sleep(TimeSpan.FromMilliseconds(100));
            Self.Tell(new PollForEvents(Context.Sender));
        }

        private void AddEventsToDocument(RequestedEvents<AtomEntry> requestedEvents)
        {
            foreach (var itemEnvelope in requestedEvents.Events)
            {
                _repository.Add((itemEnvelope.ItemToStore));
            }
            entriesInCurrentDocument += requestedEvents.Events.Count;

            if (entriesInCurrentDocument > atomDocumentSettings.NumberOfEventsPerDocument)
                currentDocumentIndex++;
        }

        private void Process(PollForEvents request)
        {
            PollSubscriptionQueue(request.AddressToPoll);
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
}
