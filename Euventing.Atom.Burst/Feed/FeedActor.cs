using System;
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

        public FeedActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
            cluster = Cluster.Get(Context.System);
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

            var documentId = new DocumentId("0");
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author,
                creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            CreateAtomDocument(documentId, creationCommand.FeedId);
        }

        private void CreateAtomDocument(DocumentId documentId, FeedId feedId)
        {
            var memberToDeployFirstDocumentOn = cluster.ReadView.Members.First();

            var props = Props.Create(() => new WorkPullingDocumentActor(new ConfigurableAtomDocumentSettings(10)));

            var atomDocument =
                Context.System.ActorOf(
                 props
                 .WithDeploy(
                     new Deploy(
                         new RemoteScope(memberToDeployFirstDocumentOn.Address))), "atomDocument_" + feedId.Id + "_" + documentId.Id);

            atomDocument.Tell(
                new CreateAtomDocumentCommand(
                    FeedTitle, FeedAuthor, feedId, documentId, null), Self);
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
