﻿using System;

namespace Eventing.InMemoryPersistence
{
    public class SnapshotEntry : IPersistableEntity
    {
        public string Id { get; }
        public string PersistenceId { get; }
        public long SequenceNr { get; }
        public DateTime Timestamp { get; }

        public object Snapshot { get; private set; }

        public SnapshotEntry(string persistenceId, long sequenceNr, DateTime timestamp, object snapshot)
        {
            this.PersistenceId = persistenceId;
            this.SequenceNr = sequenceNr;
            this.Snapshot = snapshot;
            this.Id = Guid.NewGuid().ToString();
            this.Timestamp = timestamp;
        }
    }
}
