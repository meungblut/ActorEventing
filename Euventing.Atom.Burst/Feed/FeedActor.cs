using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private int headDocumentIndex = 0;
        private List<IActorRef> documentActors = new List<IActorRef>(); 

        public FeedActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
            cluster = Cluster.Get(Context.System);
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

        private void Process(AtomFeedCreationCommand creationCommand)
        {
            var documentId = new DocumentId(creationCommand.FeedId.Id + "_" + headDocumentIndex.ToString());
            var nextDocumentId = new DocumentId(creationCommand.FeedId.Id + "_" + (headDocumentIndex + 1).ToString());
            var atomFeedCreated = new AtomFeedCreated(documentId, creationCommand.Title, creationCommand.Author,
                creationCommand.FeedId);

            Persist(atomFeedCreated, MutateInternalState);

            CreateDocumentOnEachNode(documentId, creationCommand.FeedId, nextDocumentId);
        }

        private void CreateDocumentOnEachNode(DocumentId id, FeedId feedId, DocumentId nextDocumentId)
        {
            foreach (var member in cluster.ReadView.Members)
            {
                var props =
                    Props.Create<AtomDocumentActor>(
                        () =>
                            new AtomDocumentActor(new AtomDocumentSettings(),
                                new InMemoryAtomDocumentRepository()));

                var atomDocument =
                     Context.System.ActorOf(
                         props
                         .WithDeploy(
                             new Deploy(
                                 new RemoteScope(member.Address))), "atomActor_" + member.Address.GetHashCode() + "_" + feedId.Id);

                atomDocument.Tell(
                    new CreateAtomDocumentCommand(
                        FeedTitle, FeedAuthor, feedId, id, null, nextDocumentId));

                documentActors.Add(atomDocument);

                LogTraceInfo($"Subscription Actor deployed with address {atomDocument.Path} ");
            }
        }

       private void Process(DeleteSubscriptionMessage deleteSubscription)
        {
            LogTraceInfo("Received delete subscription message");

            foreach (var currentActorRef in documentActors)
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

    internal class InMemoryAtomDocumentRepository : IAtomDocumentRepository
    {
        private readonly ConcurrentDictionary<string, PersistableAtomEntry> entries 
            = new ConcurrentDictionary<string, PersistableAtomEntry>(); 
        public void Add(PersistableAtomEntry entry)
        {
            entries.AddOrUpdate(entry.DocumentId, entry, (x, y) => entry);
        }
    }
}
