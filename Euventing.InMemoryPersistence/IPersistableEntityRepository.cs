using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Euventing.InMemoryPersistence
{
    public interface IPersistableEntityRepository
    {
        Task Save(IPersistableEntity dataToSave);
        Task<bool> Save(IEnumerable<IPersistableEntity> dataToSave);
        List<T> GetData<T>(string entityId) where T : IPersistableEntity;
        long GetMaxSequenceNumber(string entityId);
        List<T> GetData<T>(string entityId, long fromSequenceNumber, long toSequenceNumber, long maximumRowsToReturn) where T : IPersistableEntity;
        T GetData<T>(string entityId, long maximumSequenceNumber, DateTime maximumTimestamp) where T : IPersistableEntity;
        Task Delete<T>(string entityId, long toSequenceNumber, bool isPermanent);
        Task Delete<T>(string entityId, long maxSequenceNumber, DateTime maxDateTime);
        Task DeleteSingle<T>(string entityId, long sequenceNumber);
        Task Delete<T>();
    }
}