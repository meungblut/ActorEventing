using Eventing.AtomEventStore;
using Eventing.Test.Shared;
using NUnit.Framework;

namespace Eventing.EventStore.Test
{
    public class AtomEventStoreEventPublisherShould
    {
        [Test]
        public void AllowAPublishedEventToBeRetrieved()
        {
            var eventStorePublisher = new AtomEventStoreEventPublisher();
            eventStorePublisher.PublishMessage(new DummyEvent("wibs"));
        }
    }
}
