using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;

namespace Euventing.Atom.Burst.Feed
{
    public class FeedActor : AtomFeedActorBase
    {
        private readonly Cluster cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;
        private readonly ConcurrentDictionary<DocumentId, IActorRef> currentActorRefs;
        private int headDocumentIndex = 0;
        private readonly ShardedAtomFeedFactory shardedAtomFeedFactory;

        public FeedActor(IAtomDocumentSettings settings, ShardedAtomFeedFactory feedFactory)
        {
            shardedAtomFeedFactory = feedFactory;
            atomDocumentSettings = settings;
            cluster = Cluster.Get(Context.System);
            currentActorRefs = new ConcurrentDictionary<DocumentId, IActorRef>();
        }

        protected override bool ReceiveRecover(object message)
        {
            var sender = Context.Sender;

            ((dynamic)this).MutateInternalState((dynamic)message);

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            ((dynamic)this).Process((dynamic)message);

            return true;
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

        private void Process(DocumentFull documentFull)
        {
            DocumentIsFull();
        }

        private void CreateAtomDocument(DocumentId documentId, FeedId feedId, DocumentId nextDocument)
        {
            var memberToDeployFirstDocumentOn = cluster.ReadView.Members.First();

            var props = Props.Create(() => new WorkPullingDocumentActor(atomDocumentSettings, shardedAtomFeedFactory));

            var atomDocument =
                Context.System.ActorOf(
                 props
                 .WithDeploy(
                     new Deploy(
                         new RemoteScope(memberToDeployFirstDocumentOn.Address))), feedId.Id + "_" + documentId.Id);

            atomDocument.Tell(
                new CreateAtomDocumentCommand(
                    FeedTitle, FeedAuthor, feedId, documentId, null, nextDocument), Self);

            currentActorRefs.AddOrUpdate(documentId, atomDocument, (x, y) => atomDocument);
            CurrentFeedHeadDocument = documentId;
        }

        private void DocumentIsFull()
        {
            var headDocument = new DocumentId((headDocumentIndex++).ToString());
            var nextHeadDocumentId = new DocumentId((headDocumentIndex + 1).ToString());
            var addressToDeployOn = cluster.ReadView.Members.First().Address;

            var props = Props.Create(() => new WorkPullingDocumentActor(atomDocumentSettings, shardedAtomFeedFactory));

            var newActor =
                Context.System.ActorOf(
                    props.
                    WithDeploy(new Deploy(new RemoteScope(addressToDeployOn))));
            newActor.Tell(new CreateAtomDocumentCommand("", "", AtomFeedId, CurrentFeedHeadDocument, headDocument, nextHeadDocumentId));

            CurrentFeedHeadDocument = headDocument;

            currentActorRefs.AddOrUpdate(CurrentFeedHeadDocument, newActor, (x, y) => newActor);
            
            LoggingAdapter.Info($"Deployed new actor on {addressToDeployOn.Port}");
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
            LoggingAdapter.Error("Received unhandled persistence command " + unhandledMessage.GetType());
        }

        private void Process(object unhandledMessage)
        {
            LoggingAdapter.Error("Feed Actor Received unhandled command " + unhandledMessage.GetType());
        }
    }
}
