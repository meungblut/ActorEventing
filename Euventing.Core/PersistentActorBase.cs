using Akka.Event;
using Akka.Persistence;

namespace Euventing.Core
{
    public abstract class PersistentActorBase : PersistentActor
    {
        protected readonly ILoggingAdapter LoggingAdapter;
        public override string PersistenceId { get; }

        protected PersistentActorBase()
        {
            LoggingAdapter = Context.GetLogger();
            PersistenceId = Context.Parent.Path.Name + "|" + Self.Path.Name;
        }
    }
}
