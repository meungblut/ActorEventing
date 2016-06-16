using System;
using Eventing.Core.Messages;
using NLog;

namespace Eventing.Core.Logging
{
    public class LoggingEventPublisherDecorator : IEventPublisher
    {
        private readonly IEventPublisher decoratedPublisher;
        private readonly Logger logger;

        public LoggingEventPublisherDecorator(IEventPublisher decoratedPublisher)
        {
            logger = LogManager.GetLogger("Logger");
            this.decoratedPublisher = decoratedPublisher;
        }

        public void PublishMessage(DomainEvent thingToPublish)
        {
            logger.Info("IEventPublisher Publishing event " + thingToPublish.Id);
            try
            {
                decoratedPublisher.PublishMessage(thingToPublish);
                logger.Info("IEventPublisher Published event " + thingToPublish.Id);
            }
            catch (Exception exception)
            {
                logger.Error(exception.ToString());
            }
        }
    }
}
