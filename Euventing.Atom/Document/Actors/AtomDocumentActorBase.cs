using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Cluster;
using Akka.Event;
using Akka.Persistence;

namespace Euventing.Atom.Document.Actors
{
    public abstract class AtomDocumentActorBase : PersistentActor
    {
        protected string title;
        protected DateTime updated;
        protected string author;
        protected FeedId feedId;
        protected DocumentId documentId;
        protected DocumentId laterEventsDocumentId;
        protected DocumentId earlierEventsDocumentId;
        protected readonly List<AtomEntry> entries = new List<AtomEntry>();

        protected long sequenceNumber;
        protected readonly ILoggingAdapter loggingAdapter;

        public AtomDocumentActorBase()
        {
            loggingAdapter = Context.GetLogger();
            PersistenceId = "AtomDocumentActor|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        public override string PersistenceId { get; }

        protected void GetCurrentAtomDocument()
        {
            var atomDocument = new AtomDocument(title, author, feedId, documentId, earlierEventsDocumentId,
    laterEventsDocumentId, entries);
            atomDocument.AddDocumentInformation(Cluster.Get(Context.System).SelfAddress.ToString());
            Sender.Tell(atomDocument, Self);
        }
    }
}
