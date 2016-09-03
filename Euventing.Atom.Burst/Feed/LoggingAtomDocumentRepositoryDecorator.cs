using System;
using System.Threading.Tasks;
using Akka.Event;
using Eventing.Atom.Burst.Subscription;
using Eventing.Atom.Document;

namespace Eventing.Atom.Burst.Feed
{
    public class LoggingAtomDocumentRepositoryDecorator : IAtomDocumentRepository
    {
        private IAtomDocumentRepository cassandraAtomDocumentRepository;
        private ILoggingAdapter loggingAdapter;

        public LoggingAtomDocumentRepositoryDecorator(ILoggingAdapter loggingAdapter, IAtomDocumentRepository cassandraAtomDocumentRepository)
        {
            this.loggingAdapter = loggingAdapter;
            this.cassandraAtomDocumentRepository = cassandraAtomDocumentRepository;
        }

        public async Task Add(string id, AtomEntry entry)
        {
            loggingAdapter.Info($"Adding event to repository with document id {id} and id {entry.Id}");
            try
            {
                await cassandraAtomDocumentRepository.Add(id, entry);
            }
            catch (Exception e)
            {
                loggingAdapter.Error(e.ToString());
            }
        }

        public async Task<AtomDocument> GetDocument(string documentId)
        {
            loggingAdapter.Info($"Getting document with id {documentId}");
            try
            {
                var results = await cassandraAtomDocumentRepository.GetDocument(documentId);
                loggingAdapter.Info($"Got {results.Entries.Count} entries");
                return results;
            }
            catch (Exception e)
            {
                loggingAdapter.Error(e.ToString());
                throw;
            }
        }
    }
}
