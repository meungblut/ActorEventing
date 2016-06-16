using System;
using System.Collections.Generic;
using Eventing.Core;
using Eventing.Core.Messages;
using Grean.AtomEventStore;

namespace Eventing.AtomEventStore
{
    public class AtomEventStoreEventPublisher : IEventPublisher
    {
        public void PublishMessage(DomainEvent thingToPublish)
        {
            var obs = new AtomEventObserver<DomainEvent>(
                Guid.NewGuid(),
                400,
                new AtomEventsInMemory(),
                new DataContractContentSerializer(new TypeResolutionTable(new List<TypeResolutionEntry>())));

            obs.AppendAsync(thingToPublish);
        }
    }
}