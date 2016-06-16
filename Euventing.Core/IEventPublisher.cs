using Eventing.Core.Messages;

namespace Eventing.Core
{
    public interface IEventPublisher
    {
        void PublishMessage(DomainEvent thingToPublish);
    }
}