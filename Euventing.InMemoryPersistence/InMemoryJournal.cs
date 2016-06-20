using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Persistence;
using Akka.Persistence.Journal;

namespace Eventing.InMemoryPersistence
{
    public class InMemoryJournal : AsyncWriteJournal
    {
        private static IPersistableEntityRepository repository;

        static InMemoryJournal()
        {
            //repository = new LoggingPersistableEntityRepositoryDecorator(new InMemoryPersistableEntityRepository());
            repository = new InMemoryPersistableEntityRepository();
        }

        public override Task ReplayMessagesAsync(IActorContext context, string persistenceId, long fromSequenceNr, long toSequenceNr, long max,
            Action<IPersistentRepresentation> recoveryCallback)
        {
            Console.WriteLine("Replaying messages for Persistence Id:" + persistenceId);
            var persistentRepresentations = repository.GetData<JournalEntry>(persistenceId, fromSequenceNr, toSequenceNr, max);

            foreach (var persistentRepresentation in persistentRepresentations)
            {
                recoveryCallback((IPersistentRepresentation)persistentRepresentation.Payload);
            }
            return Task.FromResult(false);
        }

        public override Task<long> ReadHighestSequenceNrAsync(string persistenceId, long fromSequenceNr)
        {
            return Task.FromResult(repository.GetMaxSequenceNumber(persistenceId));
        }

        protected override Task WriteMessagesAsync(IEnumerable<Akka.Persistence.AtomicWrite> messages)
        {
            var batch = new List<JournalEntry>();
            foreach (IPersistentRepresentation message in messages)
            {
                Console.WriteLine("saving msg for Persistence Id:" + message.PersistenceId);

                batch.Add(new JournalEntry(message.PersistenceId, message.SequenceNr, message));
            }
            await repository.Save(batch);
        }

        protected override Task DeleteMessagesToAsync(string persistenceId, long toSequenceNr)
        {
            throw new NotImplementedException();
        }

        protected override async Task WriteMessagesAsync(IEnumerable<IPersistentRepresentation> messages)
        {

        }

        protected override Task DeleteMessagesToAsync(string persistenceId, long toSequenceNr, bool isPermanent)
        {
            return repository.Delete<JournalEntry>(persistenceId, toSequenceNr, isPermanent);
        }

        public static void Reset()
        {
            repository = new InMemoryPersistableEntityRepository();
        }
    }
}
