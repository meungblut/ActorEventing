using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.InMemoryPersistence
{
    public class JournalEntry : IPersistableEntity
    {
        public string Id { get; }
        public string PersistenceId { get; }
        public long SequenceNr { get; private set; }
        public DateTime Timestamp { get; }
        public object Payload { get; private set; }

        public JournalEntry(string persistenceId, long sequenceNr, object payload)
        {
            this.PersistenceId = persistenceId;
            this.SequenceNr = sequenceNr;
            this.Payload = payload;
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
