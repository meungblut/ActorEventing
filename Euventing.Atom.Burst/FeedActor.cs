using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;
using Euventing.Core;

namespace Euventing.Atom.Burst
{
    public class FeedActor : AtomFeedActorBase
    {
        private readonly Cluster cluster;

        public FeedActor()
        {
            cluster = Cluster.Get(Context.System);
        }

        protected override bool ReceiveRecover(object message)
        {
            ((dynamic) this).MutateInternalState((dynamic) message);

            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            ((dynamic) this).Process((dynamic) message);

            return true;
        }

        private void Process(AtomFeedCreationCommand creationCommand)
        {
            if (CurrentFeedHeadDocument != null)
                throw new FeedAlreadyCreatedException(CurrentFeedHeadDocument.Id);

            var documentId = new DocumentId(creationCommand.FeedId.Id + "|0");
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author,
                creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            CreateSubscriptionOnEachNode();

            CreateAtomDocument(documentId);
        }

        private void CreateAtomDocument(DocumentId documentId)
        {
            var memberToDeployFirstDocumentOn = cluster.ReadView.Members.First();
            var atomDocument =
                Context.System.ActorOf(
                 Props.Create<SubscriptionQueueActor>()
                 .WithDeploy(
                     new Deploy(
                         new RemoteScope(memberToDeployFirstDocumentOn.Address))), "atomDocument|" + documentId.Id);

            atomDocument.Tell(
                new CreateAtomDocumentCommand(
                    FeedTitle, FeedAuthor, AtomFeedId, documentId, null));
        }

        private void CreateSubscriptionOnEachNode()
        {
            foreach (var member in cluster.ReadView.Members)
            {
                var subscriptionActor =
             Context.System.ActorOf(
                 Props.Create<SubscriptionQueueActor>()
                 .WithDeploy(
                     new Deploy(
                         new RemoteScope(member.Address))), "subscription|" + AtomFeedId.Id);
            }
        }

        private void MutateInternalState(AtomFeedCreated atomFeedCreated)
        {
            AtomFeedId = atomFeedCreated.FeedId;
            CurrentFeedHeadDocument = atomFeedCreated.DocumentId;
            FeedTitle = atomFeedCreated.FeedTitle;
            FeedAuthor = atomFeedCreated.FeedAuthor;
        }
    }
}
