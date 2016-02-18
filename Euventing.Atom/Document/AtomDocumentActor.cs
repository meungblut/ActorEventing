using Akka.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Atom.Document
{
    public class AtomDocumentActor : PersistentActor
    {
        public AtomDocumentActor()
        {
            PersistenceId = Context.Parent.Path.Name + "-" + Self.Path.Name;
        }
        public string Title { get; }

        public DateTime Updated { get; }

        public string Author { get; }

        public string FeedId { get; }

        public string DocumentId { get; }

        public string LaterEventsDocumentId { get; }

        public string EarlierEventsDocumentId { get; }

        public List<AtomEntry> Entries { get; }

        protected override bool ReceiveRecover(object message)
        {
            return true;
        }

        protected override bool ReceiveCommand(object message)
        {
            return true;
        }

        public override string PersistenceId { get; }
    }
}
