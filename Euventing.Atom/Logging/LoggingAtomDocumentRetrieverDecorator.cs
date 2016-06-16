using System.Threading.Tasks;
using Eventing.Atom.Document;
using Eventing.Atom.Serialization;
using Eventing.Core.Messages;
using NLog;

namespace Eventing.Atom.Logging
{
    public class LoggingAtomDocumentRetrieverDecorator : IAtomDocumentRetriever
    {
        private readonly IAtomDocumentRetriever decoratedRetriever;
        private readonly Logger logger;

        public LoggingAtomDocumentRetrieverDecorator(IAtomDocumentRetriever decoratedRetriever)
        {
            logger = LogManager.GetLogger("logger");
            this.decoratedRetriever = decoratedRetriever;
        }

        public async Task<AtomDocument> GetDocument(DocumentId documentId)
        {
            logger.Info("GetDocument " + documentId.Id);

            try
            {
                var document = await decoratedRetriever.GetDocument(documentId);
                logger.Info(new AtomDocumentSerialiser().Serialise(document, "http://localhost:3600/events/atom/document/"));
                return document;
            }
            catch (System.Exception e)
            {
                logger.Error(e.ToString());
                throw;
            }
        }

        public async Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            logger.Info("LoggingAtomDocumentRetrieverDecorator.GetHeadDocument " + subscriptionId.Id);
            try
            {
                var document = await decoratedRetriever.GetHeadDocument(subscriptionId);
                logger.Info($"LoggingAtomDocumentRetrieverDecorator.GetHeadDocument: Returning id {document.DocumentId.Id} with {document.Entries.Count} events in");

                logger.Info(new AtomDocumentSerialiser().Serialise(document, "http://localhost:3600/events/atom/document/"));

                return document;
            }
            catch (System.Exception e)
            {
                logger.Error(e.ToString());
                throw;
            }
        }

        public Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            logger.Info("LoggingAtomDocumentRetrieverDecorator.GetHeadDocumentId " + subscriptionId.Id);

            try
            {
                return decoratedRetriever.GetHeadDocumentId(subscriptionId);
            }
            catch (System.Exception e)
            {
                logger.Error(e.ToString());
                throw;
            }
        }

        public Task<string> GetSerialisedDocument(DocumentId documentId)
        {
            logger.Info("LoggingAtomDocumentRetrieverDecorator.GetSerialisedDocument " + documentId.Id);

            try
            {
                return decoratedRetriever.GetSerialisedDocument(documentId);
            }
            catch (System.Exception e)
            {
                logger.Error(e.ToString());
                throw;
            }
        }

        public Task<string> GetSerialisedHeadDocument(SubscriptionId documentId)
        {
            logger.Info("LoggingAtomDocumentRetrieverDecorator.GetSerialisedHeadDocument " + documentId.Id);

            try
            {
                return decoratedRetriever.GetSerialisedHeadDocument(documentId);
            }
            catch (System.Exception e)
            {
                logger.Error(e.ToString());
                throw;
            }
        }
    }
}
