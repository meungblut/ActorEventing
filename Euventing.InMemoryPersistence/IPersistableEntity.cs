using System;

namespace Eventing.InMemoryPersistence
{
    public interface IPersistableEntity
    {
        string Id { get; }
        string PersistenceId { get; }
        long SequenceNr { get; }
        DateTime Timestamp { get; }
    }
}
