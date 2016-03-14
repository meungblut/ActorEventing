using Euventing.Core.Messages;

namespace Euventing.Core
{
    public interface IEventPublisher
    {
        void PublishMessage(DomainEvent thingToPublish);
    }
}