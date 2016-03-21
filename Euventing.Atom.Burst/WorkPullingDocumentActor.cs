using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using Euventing.Atom.Document;
using Euventing.Atom.Document.Actors;

namespace Euventing.Atom.Burst
{
    public class WorkPullingDocumentActor : AtomDocumentActorBase
    {
        protected Cluster Cluster;
        private readonly IAtomDocumentSettings atomDocumentSettings;

        private int entriesInCurrentDocument;

        public WorkPullingDocumentActor(IAtomDocumentSettings settings)
        {
            atomDocumentSettings = settings;
        }

        protected override void PreStart()
        {
            Cluster = Cluster.Get(Context.System);
            PollQueues();
        }

        private void PollQueues()
        {
            string addressFormat = "akka://akkaSystemName@{0}/user/subscriptionQueueActor";
            while (true)
            {
                foreach (var member in Cluster.ReadView.Members)
                {
                    var address = string.Format(addressFormat, member.Address);
                    var actorRef = Context.System.ActorSelection(addressFormat);
                    actorRef.Tell(new RequestEvents(5));
                }
            }
        }

        private void Process(IEnumerable<QueuedEvent> requestedEvents)
        {
            foreach (var requestedEvent in requestedEvents)
            {
                Persist(requestedEvent.Message, MutateInternalState);
            }

            if (entriesInCurrentDocument >= atomDocumentSettings.NumberOfEventsPerDocument)
            {
                DocumentIsFull();
            }
        }

        private void DocumentIsFull()
        {
            var documentId = new DocumentId((int.Parse(DocumentId.Id) + 1).ToString());
            var addressToDeployOn = GetDifferentNodeIfPossible();
            var newActor =
                Context.System.ActorOf(
                    Props.Create<WorkPullingDocumentActor>().WithDeploy(new Deploy(new RemoteScope(addressToDeployOn))));
            newActor.Tell(new CreateAtomDocumentCommand(this.Title, this.Author, this.FeedId, documentId, this.DocumentId));
        }

        private Address GetDifferentNodeIfPossible()
        {
            if (Cluster.ReadView.IsSingletonCluster)
                return Cluster.ReadView.SelfAddress;

            return Cluster.ReadView.Members.First(x => x.Address != Cluster.SelfAddress).Address;
        }

        private void MutateInternalState(AtomEntry entry)
        {
            Entries.Add(entry);
            entriesInCurrentDocument++;
        }

        private void Process(GetAtomDocumentRequest request)
        {
            GetCurrentAtomDocument();
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
