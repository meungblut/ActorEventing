namespace Euventing.Core.Startup
{
    public class EventSystemStartup
    {
        private IActorSystemFactory systemFactory;

        public EventSystemStartup(IActorSystemFactory actorSystemFactory)
        {
            systemFactory = actorSystemFactory;
        }

        public void Start()
        {
            ShardedActorSystemFactory factory = new ShardedActorSystemFactory(6935, "eventAkkaCLuster", "sqlite", "127.0.0.1:6935");
            factory.GetActorSystem();
        }
    }
}
