using System.Threading.Tasks;
using Akka.Persistence;
using Akka.Persistence.Snapshot;

namespace Euventing.InMemoryPersistence
{
    public class InMemorySnapshotStore : SnapshotStore
    {
        private static readonly IPersistableEntityRepository repository;

        static InMemorySnapshotStore()
        {
            repository = new InMemoryPersistableEntityRepository();
        }
        //public InMemorySnapshotStore()
        //{
        //    //repository = new LoggingPersistableEntityRepositoryDecorator(new InMemoryPersistableEntityRepository());
        //    repository = new InMemoryPersistableEntityRepository();
        //}

        protected override async Task<SelectedSnapshot> LoadAsync(string persistenceId, SnapshotSelectionCriteria criteria)
        {
            var snapshotEntry = repository.GetData<SnapshotEntry>(persistenceId, criteria.MaxSequenceNr, criteria.MaxTimeStamp);

            var snapshot = new SelectedSnapshot(new SnapshotMetadata(snapshotEntry.PersistenceId, snapshotEntry.SequenceNr, snapshotEntry.Timestamp), snapshotEntry.Snapshot);

            return snapshot;
        }

        protected override async Task SaveAsync(SnapshotMetadata metadata, object snapshot)
        {
            await repository.Save(new SnapshotEntry(metadata.PersistenceId, metadata.SequenceNr, metadata.Timestamp, snapshot));
        }

        protected override void Saved(SnapshotMetadata metadata)
        {
        }

        protected override async Task DeleteAsync(SnapshotMetadata metadata)
        {
            await repository.DeleteSingle<SnapshotEntry>(metadata.PersistenceId, metadata.SequenceNr);
        }

        protected override async Task DeleteAsync(string persistenceId, SnapshotSelectionCriteria criteria)
        {
            await repository.Delete<SnapshotEntry>(persistenceId, criteria.MaxSequenceNr, criteria.MaxTimeStamp);
        }
    }
}
