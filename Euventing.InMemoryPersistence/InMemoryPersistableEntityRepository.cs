using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Euventing.InMemoryPersistence
{
    public class InMemoryPersistableEntityRepository : IPersistableEntityRepository
    {
        private Dictionary<string, List<IPersistableEntity>> _dataToStore = new Dictionary<string, List<IPersistableEntity>>();

        public Task Save(IPersistableEntity dataToSave)
        {
            if (!_dataToStore.ContainsKey(dataToSave.PersistenceId))
                _dataToStore.Add(dataToSave.PersistenceId, new List<IPersistableEntity>());

            _dataToStore[dataToSave.PersistenceId].Add(dataToSave);

            return Task.FromResult(true);
        }

        public Task<bool> Save(IEnumerable<IPersistableEntity> dataToSave)
        {
            foreach (var data in dataToSave)
            {
                if (!_dataToStore.ContainsKey(data.PersistenceId))
                    _dataToStore.Add(data.PersistenceId, new List<IPersistableEntity>());

                _dataToStore[data.PersistenceId].Add(data);
            }
            return Task.FromResult(true);
        }

        public List<T> GetData<T>(string entityId) where T : IPersistableEntity
        {
            return _dataToStore[entityId].ConvertAll(new Converter<IPersistableEntity, T>(x => (T)x));
        }

        public List<T> GetData<T>() where T : IPersistableEntity
        {
            return _dataToStore.First().Value.ConvertAll(new Converter<IPersistableEntity, T>(x => (T)x));
        }

        public long GetMaxSequenceNumber(string entityId)
        {
            if (!_dataToStore.ContainsKey(entityId))
                return 0;

            return _dataToStore[entityId].Select(x => x.SequenceNr).Max();
        }

        public List<T> GetData<T>(string entityId, long fromSequenceNumber, long toSequenceNumber, long maximumRowsToReturn) where T : IPersistableEntity
        {
            int rowsToTake = 0;
            if (maximumRowsToReturn == long.MaxValue)
                rowsToTake = int.MaxValue;
            else
                rowsToTake = (int)maximumRowsToReturn;

            if (!_dataToStore.ContainsKey(entityId))
                return new List<T>();

            var data = _dataToStore[entityId].Where(x => x.SequenceNr >= fromSequenceNumber && x.SequenceNr <= toSequenceNumber && x.GetType() == typeof(T)).Take(rowsToTake).ToList().ConvertAll(new Converter<IPersistableEntity, T>(x => (T)x));
            return data;
        }

        public T GetData<T>(string entityId, long maximumSequenceNumber, DateTime maximumTimestamp) where T : IPersistableEntity
        {
            if (!_dataToStore.ContainsKey(entityId))
                return default(T);

            var data =
               (T)_dataToStore[entityId].LastOrDefault(x => x.SequenceNr <= maximumSequenceNumber && x.Timestamp <= maximumTimestamp && x.GetType() == typeof(T));
            return data;
        }

        public async Task Delete<T>(string entityId, long toSequenceNumber, bool isPermanent)
        {
            if (isPermanent)
            {
                _dataToStore[entityId].RemoveAll(x => x.SequenceNr <= toSequenceNumber && x.GetType() == typeof(T));
                return;
            }

            var data = _dataToStore[entityId].Where(x => x.SequenceNr <= toSequenceNumber && x.GetType() == typeof(T)).ToList().ConvertAll(new Converter<IPersistableEntity, JournalEntry>(x => (JournalEntry)x));
            foreach (var journalEntry in data)
            {
                journalEntry.Payload.GetType().GetProperty("IsDeleted").SetValue(journalEntry.Payload, true, null);
            }
        }

        public async Task DeleteSingle<T>(string entityId, long sequenceNumber)
        {
            _dataToStore[entityId].RemoveAll(x => x.SequenceNr == sequenceNumber && x.GetType() == typeof(T));
        }

        public async Task Delete<T>()
        {
            return;
        }

        public async Task Delete<T>(string entityId, long maxSequenceNumber, DateTime maxDateTime)
        {
            _dataToStore[entityId].RemoveAll(x => x.SequenceNr <= maxSequenceNumber && x.Timestamp <= maxDateTime && x.GetType() == typeof(T));
        }
    }
}