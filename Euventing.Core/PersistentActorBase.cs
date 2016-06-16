using Akka.Event;
using Akka.Persistence;

namespace Eventing.Core
{
    public abstract class PersistentActorBase : PersistentActor
    {
        protected readonly ILoggingAdapter LoggingAdapter;
        public override string PersistenceId { get; }

        protected PersistentActorBase()
        {
            LoggingAdapter = Context.GetLogger();
            PersistenceId = this.GetType() + "|" + Context.Parent.Path.Name + "|" + Self.Path.Name;
            LogTraceInfo("Constructed");
        }

        protected void LogTraceInfo(string dataToLog)
        {
            if (LoggingAdapter.IsInfoEnabled)
                LoggingAdapter.Info(GetFullInfoString(dataToLog));
        }

        protected void LogError(string dataToLog)
        {
            LoggingAdapter.Error(GetFullInfoString(dataToLog));
        }

        private string GetFullInfoString(string dataToLog)
        {
            return $"PersistId: {PersistenceId}, in type {this.GetType()} in instance {this.GetHashCode()} : {dataToLog}";
        }
    }
}
