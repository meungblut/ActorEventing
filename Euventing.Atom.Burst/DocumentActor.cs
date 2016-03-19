using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;
using Akka.Persistence;
using Euventing.Atom.Document;

namespace Euventing.Atom.Burst
{
    public class DocumentActor : PersistentActor
    {
        protected Cluster Cluster;

        private DocumentId documentId;
        private DocumentId laterEventsDocumentId;
        private DocumentId earlierEventsDocumentId;
        private int maximumEventsInDocument;

        private readonly List<AtomEntry> entries = new List<AtomEntry>();

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

        protected override bool ReceiveRecover(object message)
        {
            throw new NotImplementedException();
        }

        protected override bool ReceiveCommand(object message)
        {
            throw new NotImplementedException();
        }

        public override string PersistenceId { get; }

        private void DocumentFull()
        {
            
        }
    }
}
