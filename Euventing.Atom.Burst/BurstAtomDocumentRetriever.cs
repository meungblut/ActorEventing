using System;
using System.Threading.Tasks;
using Akka.Actor;
using Euventing.Atom.Burst.Feed;
using Euventing.Atom.Burst.Subscription;
using Euventing.Atom.Document;
using Euventing.Core.Messages;
using Euventing.Atom.Serialization;

namespace Euventing.Atom.Burst
{
    public class BurstAtomDocumentRetriever : IAtomDocumentRetriever
    {
        private readonly BurstSubscriptionManager burstManager;
        private readonly AtomDocumentSerialiser atomDocumentSerialiser;

        public BurstAtomDocumentRetriever(BurstSubscriptionManager burstManager)
        {
            atomDocumentSerialiser = new AtomDocumentSerialiser();
            this.burstManager = burstManager;
        }

        public async Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            return null;
            //var documentId = await shardedAtomFeedFactory.GetActorRef(null).Ask<DocumentId>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            //return documentId;
        }

        public async Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            var document = await burstManager.BurstSubscriptionActorRef.Ask<AtomDocument>(new GetHeadDocumentForFeedRequest(subscriptionId));
            return document;
        }

        public async Task<AtomDocument> GetDocument(DocumentId documentId)
        {
            throw new NotImplementedException();
            //var documentId = await shardedAtomFeedFactory.GetActorRef().Ask<IActorRef>(new GetHeadDocumentIdForFeedRequest(subscriptionId));
            //var atomDocument = await documentId.Ask<AtomDocument>(new GetAtomDocumentRequest());
            //return atomDocument;
        }

        public async Task<string> GetSerialisedDocument(DocumentId documentId)
        {
            throw new NotImplementedException();

            //var atomDocument = await shardedAtomDocumentFactory.GetActorRef().Ask<AtomDocument>(new GetAtomDocumentRequest(documentId));
            //return atomDocumentSerialiser.Serialise(atomDocument, "http://matt.com");
        }

        public async Task<string> GetSerialisedHeadDocument(SubscriptionId documentId)
        {
            var atomDocument = await GetHeadDocument(documentId);
            return atomDocumentSerialiser.Serialise(atomDocument, "http://matt.com");
        }
    }
}
