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
        private IAtomDocumentSettings atomDocumentSettings;
        private readonly ConcurrentDictionary<DocumentId, IActorRef> currentActorRefs;
        private DocumentId headDocumentId;
        private int headDocumentIndex = 0;

        public FeedActor(IAtomDocumentSettings settings)
        {
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

            var documentId = new DocumentId(creationCommand.FeedId.Id + "|" + headDocumentIndex);
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author,
                creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            CreateAtomDocument(documentId, creationCommand.FeedId);
        }

        private void Process(GetHeadDocumentIdForFeedRequest getHeadDocumentIdForFeedRequest)
        {
            Sender.Tell(currentActorRefs[CurrentFeedHeadDocument]);
        }

        private void CreateAtomDocument(DocumentId documentId, FeedId feedId)
        {
            var memberToDeployFirstDocumentOn = cluster.ReadView.Members.First();

            var props = Props.Create(() => new WorkPullingDocumentActor(new ConfigurableAtomDocumentSettings(1000)));

            var atomDocument =
                Context.System.ActorOf(
                 props
                 .WithDeploy(
                     new Deploy(
                         new RemoteScope(memberToDeployFirstDocumentOn.Address))), feedId.Id + "|" + documentId.Id);

            atomDocument.Tell(
                new CreateAtomDocumentCommand(
                    FeedTitle, FeedAuthor, feedId, documentId, null), Self);

            currentActorRefs.AddOrUpdate(documentId, atomDocument, (x, y) => atomDocument);
            headDocumentId = documentId;
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
            LoggingAdapter.Error("Received unhandled persistence command " + unhandledMessage.GetType().ToString());
        }

        private void Process(object unhandledMessage)
        {
            LoggingAdapter.Error("Received unhandled command " + unhandledMessage.GetType().ToString());
        }
    }
}
