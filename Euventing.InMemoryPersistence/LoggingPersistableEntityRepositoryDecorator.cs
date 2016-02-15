using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Euventing.InMemoryPersistence
{
    public class LoggingPersistableEntityRepositoryDecorator : IPersistableEntityRepository
    {
        private readonly IPersistableEntityRepository persistableEntityRepository;
        private readonly JsonSerializerSettings jsonSerializerSettings;

        public LoggingPersistableEntityRepositoryDecorator(IPersistableEntityRepository decoratedRepository)
        {
            persistableEntityRepository = decoratedRepository;
            jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new PropertyIgnoringCamelCaseResolver(),
                TypeNameHandling = TypeNameHandling.All,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        public Task Save(IPersistableEntity dataToSave)
        {
            Log("Saving single ", dataToSave);
            try
            {
                return persistableEntityRepository.Save(dataToSave);
            }
            catch (Exception e)
            {
                Log("couldn't save", e);
                throw;
            }
        }

        public Task<bool> Save(IEnumerable<IPersistableEntity> dataToSave)
        {
            Log("Saving multiple ", dataToSave);
            try
            {
                return persistableEntityRepository.Save(dataToSave);
            }
            catch (Exception e)
            {
                Log("couldn't save", e);
                throw;
            }
        }

        public List<T> GetData<T>(string entityId) where T : IPersistableEntity
        {
            Log("GetData by entityId ", entityId);
            var data = persistableEntityRepository.GetData<T>(entityId);
            Log("Got data : ", data);
            return data;
        }

        public long GetMaxSequenceNumber(string entityId)
        {
            Log("GetMaxSequenceNumber ", entityId);
            var data = persistableEntityRepository.GetMaxSequenceNumber(entityId);
            Log("Got data : ", data);
            return data;
        }

        public List<T> GetData<T>(string entityId, long fromSequenceNumber, long toSequenceNumber, long maximumRowsToReturn) where T : IPersistableEntity
        {
            Log("GetData ", entityId, fromSequenceNumber, toSequenceNumber, maximumRowsToReturn);
            List<T> data;
            try
            {
                data = persistableEntityRepository.GetData<T>(entityId, fromSequenceNumber, toSequenceNumber,
                    maximumRowsToReturn);
            }
            catch (Exception e)
            {
                Log("Error retrieving data ", e);
                throw;
            }
            Log("Got data : ", data);
            return data;
        }

        public T GetData<T>(string entityId, long maximumSequenceNumber, DateTime maximumTimestamp) where T : IPersistableEntity
        {
            Log("GetData by sequence and timestamp", entityId, maximumSequenceNumber, maximumTimestamp);
            try
            {
                var data = persistableEntityRepository.GetData<T>(entityId, maximumSequenceNumber, maximumTimestamp);
                Log("Got data : ", data);
                return data;
            }
            catch (Exception e)
            {
                Log("Error retrievingdata ", e);
                throw;
            }
        }

        public Task Delete<T>(string entityId, long toSequenceNumber, bool isPermanent)
        {
            Log("Delete by tosequenceNumber", entityId, toSequenceNumber, isPermanent);
            return persistableEntityRepository.Delete<T>(entityId, toSequenceNumber, isPermanent);
        }

        public Task Delete<T>(string entityId, long maxSequenceNumber, DateTime maxDateTime)
        {
            Log("Delete ", entityId, maxSequenceNumber, maxDateTime);
            return persistableEntityRepository.Delete<T>(entityId, maxSequenceNumber, maxDateTime);
        }

        public Task DeleteSingle<T>(string entityId, long sequenceNumber)
        {
            Log("Delete by tosequenceNumber", entityId, sequenceNumber);
            return persistableEntityRepository.DeleteSingle<T>(entityId, sequenceNumber);
        }

        public Task Delete<T>()
        {
            return persistableEntityRepository.Delete<T>();
        }

        void Log(string contents, params object[] data)
        {
            Console.WriteLine();
            Console.WriteLine(DateTime.Now.ToString("yy-mm-dd hh:M:ss fff ") + contents + " " + JsonConvert.SerializeObject(data, jsonSerializerSettings));
            Console.WriteLine();
        }
    }
}
