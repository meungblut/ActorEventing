using Akka.Cluster.Sharding;
using Akka.Event;

namespace Euventing.Atom.Burst.Subscription
{
    public class LoggingMessageExtractorDecorator : IMessageExtractor
    {
        private readonly IMessageExtractor _messageExtractor;
        private readonly ILoggingAdapter _adapter;

        public LoggingMessageExtractorDecorator(IMessageExtractor messageExtractor, ILoggingAdapter adapter)
        {
            _adapter = adapter;
            _messageExtractor = messageExtractor;
        }

        public string EntityId(object message)
        {
            _adapter.Log(LogLevel.InfoLevel,  $"Getting entity id for type {message.GetType()} from {_messageExtractor.GetType()}");
            var result = _messageExtractor.EntityId(message);
            _adapter.Log(LogLevel.InfoLevel, $"Entity id was {result}");
            return result;
        }

        public object EntityMessage(object message)
        {
            _adapter.Log(LogLevel.InfoLevel, $"Getting entity message for type {message.GetType()}");
            var result = _messageExtractor.EntityMessage(message);
            _adapter.Log(LogLevel.InfoLevel, $"Entity message was {result}");
            return result;
        }

        public string ShardId(object message)
        {
            _adapter.Log(LogLevel.InfoLevel, $"Getting shard id for type {message.GetType()}");

            if (message is GetHeadDocumentForFeedRequest)
                _adapter.Log(LogLevel.InfoLevel, $"Getting shard id for GetHeadDocForFeedRequest '{((GetHeadDocumentForFeedRequest)message).SubscriptionId.Id}'");


            var result = _messageExtractor.ShardId(message);
            _adapter.Log(LogLevel.InfoLevel, $"Shard id was {result}");
            return result;
        }
    }
}
