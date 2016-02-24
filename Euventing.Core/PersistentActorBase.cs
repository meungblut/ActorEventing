using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Event;
using Akka.Persistence;

namespace Euventing.Core
{
    public abstract class PersistentActorBase : PersistentActor
    {
        private readonly ILoggingAdapter loggingAdapter;
        public override string PersistenceId { get; }


        protected PersistentActorBase()
        {
            loggingAdapter = Context.GetLogger();
            PersistenceId = Context.Parent.Path.Name + "|" + Self.Path.Name;
        }

        protected override bool ReceiveCommand(object message)
        {
            if (message == null)
            {
                Console.WriteLine("Received null message");
                return true;
            }

            try
            {
                ((dynamic)this).Process((dynamic)message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error processing " + message.GetType() + " " + e.ToString());
            }

            return true;
        }

        protected void Process(object unknownCommand)
        {
            loggingAdapter.Error("Received Unknown command: " + unknownCommand.GetType().ToString());
        }

        protected void MutateInternalState(object unknownRecoveryCommand)
        {
            loggingAdapter.Error("Received Unknown recovery command: " + unknownRecoveryCommand.GetType().ToString());
        }
    }
}
