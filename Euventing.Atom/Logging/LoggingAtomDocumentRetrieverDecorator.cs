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

        public Task<AtomDocument> GetHeadDocument(SubscriptionId subscriptionId)
        {
            logger.Info("GetHeadDocument " + subscriptionId.Id);
            try
            {
                return decoratedRetriever.GetHeadDocument(subscriptionId);
            }
            catch (System.Exception e)
            {
                logger.Error(e.ToString());
                throw;
            }
        }

        public Task<DocumentId> GetHeadDocumentId(SubscriptionId subscriptionId)
        {
            logger.Info("GetHeadDocumentId " + subscriptionId.Id);

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
            logger.Info("GetSerialisedDocument " + documentId.Id);

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
            logger.Info("GetSerialisedHeadDocument " + documentId.Id);

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
