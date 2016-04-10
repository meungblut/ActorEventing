using System.Threading.Tasks;
using Euventing.Atom.Document;
using Euventing.Core.Messages;
using NLog;

namespace Euventing.Atom.Logging
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

        public Task<AtomDocument> GetDocument(DocumentId documentId)
        {
            logger.Info("GetDocument " + documentId.Id);

            try
            {
                return decoratedRetriever.GetDocument(documentId);
            }
            catch (System.Exception e)
            {
                logger.Error(e.ToString());
                throw;
            }
        }

        public async Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            if (subscriptionId.Id == "6985769857")
                logger.Info("Double down");

            logger.Info("LoggingAtomDocumentRetrieverDecorator.GetHeadDocument " + subscriptionId.Id);
            try
            {
                var document = await decoratedRetriever.GetHeadDocument(subscriptionId);
                logger.Info($"LoggingAtomDocumentRetrieverDecorator.GetHeadDocument: Returning id {document.DocumentId.Id} with {document.Entries.Count} events in");
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
