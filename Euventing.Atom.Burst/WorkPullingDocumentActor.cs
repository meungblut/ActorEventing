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

namespace Euventing.Atom.Burst
{
    public class WorkPullingDocumentActor : AtomDocumentActorBase
    {
        protected Cluster Cluster;

        protected override void PreStart()
        {
            Cluster = Cluster.Get(Context.System);
            PollQueues();
        }

        private void PollQueues()
        {
            string addressFormat = "akka://akkaSystemName@{0}/user/localEventQueue";
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
                
            }
        }

        private void Process(GetAtomDocumentRequest request)
        {
            loggingAdapter.Debug("Request for document id {0} on node {2} with events {3}",
     PersistenceId, Cluster.Get(Context.System).SelfAddress, entries.Count);

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

        public override string PersistenceId { get; }

        private void DocumentFull()
        {
            
        }
    }
}
