using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.InMemoryPersistence
{
    public interface IPersistableEntity
    {
        string Id { get; }
        string PersistenceId { get; }
        long SequenceNr { get; }
        DateTime Timestamp { get; }
    }
}
