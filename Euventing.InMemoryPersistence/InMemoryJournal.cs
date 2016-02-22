using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Persistence;
using Akka.Persistence.Journal;

namespace Euventing.InMemoryPersistence
{
    public class InMemoryJournal : AsyncWriteJournal
    {
        private static IPersistableEntityRepository repository;

        static InMemoryJournal()
        {
            repository = new InMemoryPersistableEntityRepository();
        }

        //public InMemoryJournal()
        //{
        //    //repository = new LoggingPersistableEntityRepositoryDecorator(new InMemoryPersistableEntityRepository());
        //    repository = new InMemoryPersistableEntityRepository();
        //}

        public override Task ReplayMessagesAsync(string persistenceId, long fromSequenceNr, long toSequenceNr, long max, Action<IPersistentRepresentation> replayCallback)
        {
            Console.WriteLine("Replaying messages for Persistence Id:" + persistenceId);
            var persistentRepresentations = repository.GetData<JournalEntry>(persistenceId, fromSequenceNr, toSequenceNr, max);

            foreach (var persistentRepresentation in persistentRepresentations)
            {
                replayCallback((IPersistentRepresentation)persistentRepresentation.Payload);
            }
            return Task.FromResult(false);
        }

        public override Task<long> ReadHighestSequenceNrAsync(string persistenceId, long fromSequenceNr)
        {
            return Task.FromResult(repository.GetMaxSequenceNumber(persistenceId));
        }

        protected override async Task WriteMessagesAsync(IEnumerable<IPersistentRepresentation> messages)
        {
            var batch = new List<JournalEntry>();
            foreach (IPersistentRepresentation message in messages)
            {
                Console.WriteLine("saving msg for Persistence Id:" + message.PersistenceId);

                batch.Add(new JournalEntry(message.PersistenceId, message.SequenceNr, message));
            }
            await repository.Save(batch);
        }

        protected override Task DeleteMessagesToAsync(string persistenceId, long toSequenceNr, bool isPermanent)
        {
            return repository.Delete<JournalEntry>(persistenceId, toSequenceNr, isPermanent);
        }
    }
}
