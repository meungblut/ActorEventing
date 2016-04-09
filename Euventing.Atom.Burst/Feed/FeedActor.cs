using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core.Messages;

namespace Euventing.Atom.Burst.Feed
{
    public class FeedActor : AtomFeedActorBase
    {
        private readonly Cluster cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;
        private readonly ConcurrentDictionary<DocumentId, IActorRef> currentActorRefs;
        private int headDocumentIndex = 0;
        private SubscriptionsAtomFeedShouldPoll subscriptionsAtomFeedShouldPoll;

        public FeedActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
            cluster = Cluster.Get(Context.System);
            currentActorRefs = new ConcurrentDictionary<DocumentId, IActorRef>();
        }

        protected override bool ReceiveRecover(object message)
        {
            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            ((dynamic)this).Process((dynamic)message);

            return true;
        }

        private void Process(SubscriptionsAtomFeedShouldPoll subscriptions)
        {
            subscriptionsAtomFeedShouldPoll = subscriptions;
        }

        private void Process(AtomFeedCreationCommand creationCommand)
        {
            if (CurrentFeedHeadDocument != null)
                throw new FeedAlreadyCreatedException(CurrentFeedHeadDocument.Id);

            var documentId = new DocumentId(headDocumentIndex.ToString());
            var nextDocumentId = new DocumentId((headDocumentIndex + 1).ToString());
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author,
                creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            CreateAtomDocument(documentId, creationCommand.FeedId, nextDocumentId);
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadDocumentIdForFeedRequest)
        {
            Sender.Tell(currentActorRefs[CurrentFeedHeadDocument]);
        }

        private async void Process(GetHeadDocumentForFeedRequest getHeadDocumentIdForFeedRequest)
        {
            Context.GetLogger().Info("Getting atom document");
            currentActorRefs[CurrentFeedHeadDocument].Forward(new GetAtomDocumentRequest());
        }

        private void Process(DocumentFull documentFull)
        {
            DocumentIsFull();
        }

        private void CreateAtomDocument(DocumentId documentId, FeedId feedId, DocumentId nextDocument)
        {
            var memberToDeployFirstDocumentOn = cluster.ReadView.Members.First();

            var props = Props.Create(() => new WorkPullingDocumentActor(atomDocumentSettings));

            var atomDocument =
                Context.ActorOf(
                 props
                 .WithDeploy(
                     new Deploy(
                         new RemoteScope(memberToDeployFirstDocumentOn.Address))), feedId.Id + "_" + documentId.Id);

            atomDocument.Tell(
                new CreateAtomDocumentCommand(
                    FeedTitle, FeedAuthor, feedId, documentId, null, nextDocument), Self);

            atomDocument.Tell(subscriptionsAtomFeedShouldPoll);

            currentActorRefs.AddOrUpdate(documentId, atomDocument, (x, y) => atomDocument);
            CurrentFeedHeadDocument = documentId;
        }

        private void DocumentIsFull()
        {
            var headDocument = new DocumentId((headDocumentIndex++).ToString());
            var nextHeadDocumentId = new DocumentId((headDocumentIndex + 1).ToString());
            var addressToDeployOn = cluster.ReadView.Members.First().Address;

            var props = Props.Create(() => new WorkPullingDocumentActor(atomDocumentSettings));

            var newActor =
                Context.ActorOf(
                    props.
                    WithDeploy(new Deploy(new RemoteScope(addressToDeployOn))));

            newActor.Tell(new CreateAtomDocumentCommand("", "", AtomFeedId, CurrentFeedHeadDocument, headDocument, nextHeadDocumentId));
            newActor.Tell(subscriptionsAtomFeedShouldPoll);

            CurrentFeedHeadDocument = headDocument;

            currentActorRefs.AddOrUpdate(CurrentFeedHeadDocument, newActor, (x, y) => newActor);

            Context.GetLogger().Info($"Deployed new actor on {addressToDeployOn.Port}");
        }

        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            foreach (var currentActorRef in currentActorRefs.Values)
            {
                currentActorRef.Tell(deleteSubscription);
            }
        }

        private void MutateInternalState(AtomFeedCreated atomFeedCreated)
        {
            AtomFeedId = atomFeedCreated.FeedId;
            CurrentFeedHeadDocument = atomFeedCreated.DocumentId;
            FeedTitle = atomFeedCreated.FeedTitle;
            FeedAuthor = atomFeedCreated.FeedAuthor;
        }

        private void MutateInternalState(object unhandledMessage)
        {
            Context.GetLogger().Error("Received unhandled persistence command " + unhandledMessage.GetType());
        }

        private void Process(object unhandledMessage)
        {
            Context.GetLogger().Error("Feed Actor Received unhandled command " + unhandledMessage.GetType());
        }
    }
}
