using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Cassandra;
using CassandraRepository;
using Eventing.Atom.Burst.Subscription;
using Eventing.Atom.Document;

namespace Eventing.Atom.Burst.Feed
{
    public class CassandraAtomDocumentRepository : IAtomDocumentRepository
    {
        private readonly SingletonCassandraSessionFactory _sessionFactory;
        private ObjectStore repo;

        public CassandraAtomDocumentRepository(SingletonCassandraSessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
            repo = new ObjectStore(_sessionFactory);
        }

        public async Task Add(string id, AtomEntry entry)
        {
            await repo.Save(id, Guid.NewGuid().ToString(), entry, ConsistencyLevel.Any);
        }

        public async Task<AtomDocument> GetDocument(string documentId)
        {
            var results = await repo.GetAll(documentId, ConsistencyLevel.One);
            List<AtomEntry> atomEntries = new List<AtomEntry>();

            foreach (var result in results)
            {
                atomEntries.Add((AtomEntry)result);
            }
            var document = new AtomDocument("", "", new FeedId(""), new DocumentId(documentId), new DocumentId(documentId), atomEntries);
            return document;
        }
    }
}
