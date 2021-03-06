﻿using System;
using System.Collections.Generic;
using Akka.Cluster;
using Eventing.Core;

namespace Eventing.Atom.Document.Actors
{
    public abstract class AtomDocumentActorBase : PersistentActorBase
    {
        protected string Title;
        protected DateTime Updated;
        protected string Author;
        protected FeedId FeedId;
        protected DocumentId DocumentId;
        protected DocumentId LaterEventsDocumentId;
        protected DocumentId EarlierEventsDocumentId;
        protected readonly List<AtomEntry> Entries = new List<AtomEntry>();

        protected long SequenceNumber;

        protected void GetCurrentAtomDocument()
        {
            var atomDocument = new AtomDocument(Title, Author, FeedId, DocumentId, LaterEventsDocumentId, Entries);
            atomDocument.AddDocumentInformation(Cluster.Get(Context.System).SelfAddress.ToString());
            Sender.Tell(atomDocument, Self);
        }
    }
}
