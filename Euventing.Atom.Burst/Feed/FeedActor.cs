using System.Collections.Concurrent;
using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
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
        private readonly ConcurrentDictionary<DocumentId, IActorRef> currentAtomDocumentActorRefs;
        private int headDocumentIndex = 0;
        private SubscriptionsAtomFeedShouldPoll subscriptionsAtomFeedShouldPoll;

        public FeedActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
            cluster = Cluster.Get(Context.System);
            currentAtomDocumentActorRefs = new ConcurrentDictionary<DocumentId, IActorRef>();
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
            var documentId = new DocumentId(creationCommand.FeedId.Id + "_" + headDocumentIndex.ToString());
            var nextDocumentId = new DocumentId(creationCommand.FeedId.Id + "_" + (headDocumentIndex + 1).ToString());
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author,
                creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            CreateAtomDocument(documentId, creationCommand.FeedId, nextDocumentId);
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadDocumentIdForFeedRequest)
        {
            Sender.Tell(currentAtomDocumentActorRefs[CurrentFeedHeadDocument]);
        }

        private void Process(GetDocumentFromFeedRequest getDocumentRequest)
        {
            LogTraceInfo($"Asking for document with id {getDocumentRequest.DocumentId.Id}");

            var actor = currentAtomDocumentActorRefs[getDocumentRequest.DocumentId];
            actor.Forward(new GetAtomDocumentRequest());
        }

        private void Process(GetHeadDocumentForFeedRequest getHeadDocumentIdForFeedRequest)
        {
            LogTraceInfo($"Getting atom document {getHeadDocumentIdForFeedRequest} feed actor");
            currentAtomDocumentActorRefs[CurrentFeedHeadDocument].Forward(new GetAtomDocumentRequest());
        }

        private void Process(DocumentFull documentFull)
        {
            LogTraceInfo($"Received document full {documentFull.DocumentId.Id}");

            DocumentIsFull(documentFull);
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

            currentAtomDocumentActorRefs.AddOrUpdate(documentId, atomDocument, (x, y) => atomDocument);
            CurrentFeedHeadDocument = documentId;
        }

        private void DocumentIsFull(DocumentFull documentFull)
        {
            if (CurrentFeedHeadDocument != documentFull.DocumentId)
            {
                LogTraceInfo($"Received document full for non-head document {documentFull.DocumentId}");
            }

            var outgoingHeadDocument = CurrentFeedHeadDocument;
            var incomingHeadDocument = new DocumentId(AtomFeedId.Id + "_" + (++headDocumentIndex));
            var headAfterIncomingHEadDocumentId = new DocumentId(AtomFeedId.Id + "_" + (headDocumentIndex + 1));

            var addressToDeployOn = cluster.ReadView.Members.First().Address;

            var props = Props.Create(() => new WorkPullingDocumentActor(atomDocumentSettings));

            var newActor =
                Context.ActorOf(
                    props.
                    WithDeploy(new Deploy(new RemoteScope(addressToDeployOn))), incomingHeadDocument.Id);

            newActor.Tell(new CreateAtomDocumentCommand("", "", AtomFeedId, incomingHeadDocument, outgoingHeadDocument, headAfterIncomingHEadDocumentId));
            newActor.Tell(subscriptionsAtomFeedShouldPoll);

            CurrentFeedHeadDocument = incomingHeadDocument;

            currentAtomDocumentActorRefs.AddOrUpdate(CurrentFeedHeadDocument, newActor, (x, y) => newActor);

            LogTraceInfo($"Deployed new document actor with id {incomingHeadDocument.Id} on {addressToDeployOn.Port}");

            LogTraceInfo($"Head document is now {CurrentFeedHeadDocument.Id}");
        }

        private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            LogTraceInfo("Received delete subscription message");

            foreach (var currentActorRef in currentAtomDocumentActorRefs.Values)
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

        private void MutateInternalState(RecoveryCompleted recoveryCompleted)
        {
            UnstashAll();
        }

        private void MutateInternalState(object unhandledMessage)
        {
            LogTraceInfo("Received unhandled persistence command " + unhandledMessage.GetType());
        }

        private void Process(object unhandledMessage)
        {
            LogTraceInfo("Feed Actor Received unhandled command " + unhandledMessage.GetType());
        }
    }
}
