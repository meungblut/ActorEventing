using System;
using System.Threading.Tasks;
using Akka.Persistence;
using Akka.Persistence.Snapshot;

namespace Euventing.InMemoryPersistence
{
    public class InMemorySnapshotStore : SnapshotStore
    {
        private static IPersistableEntityRepository repository;

        public static IPersistableEntityRepository RepositorySavedWIth { get { return repository; } }

        static InMemorySnapshotStore()
        {
            repository = new InMemoryPersistableEntityRepository();
        }

        protected override async Task<SelectedSnapshot> LoadAsync(string persistenceId, SnapshotSelectionCriteria criteria)
        {
            var snapshotEntry = repository.GetData<SnapshotEntry>(persistenceId, criteria.MaxSequenceNr, criteria.MaxTimeStamp);

            if (snapshotEntry == null)
                return (SelectedSnapshot) null;

            var snapshot = new SelectedSnapshot(new SnapshotMetadata(snapshotEntry.PersistenceId, snapshotEntry.SequenceNr, snapshotEntry.Timestamp), snapshotEntry.Snapshot);
            return snapshot;
        }

        protected override async Task SaveAsync(SnapshotMetadata metadata, object snapshot)
        {
            Console.WriteLine("Saving snapshot");
            await repository.Save(new SnapshotEntry(metadata.PersistenceId, metadata.SequenceNr, metadata.Timestamp, snapshot));
        }

        protected override void Saved(SnapshotMetadata metadata)
        {
            Console.WriteLine("Saving snapshot");
            repository.Save(new SnapshotEntry(metadata.PersistenceId, metadata.SequenceNr, metadata.Timestamp, null));
        }

        protected override async Task DeleteAsync(SnapshotMetadata metadata)
        {
            await repository.DeleteSingle<SnapshotEntry>(metadata.PersistenceId, metadata.SequenceNr);
        }

        protected override async Task DeleteAsync(string persistenceId, SnapshotSelectionCriteria criteria)
        {
            await repository.Delete<SnapshotEntry>(persistenceId, criteria.MaxSequenceNr, criteria.MaxTimeStamp);
        }

        public static void Reset()
        {
            repository = new InMemoryPersistableEntityRepository();
        }
    }
}
