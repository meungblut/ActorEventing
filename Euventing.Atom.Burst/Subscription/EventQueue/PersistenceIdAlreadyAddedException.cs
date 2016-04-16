using System;

namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    public class PersistenceIdAlreadyAddedException : Exception
    {
        public long PersistenceReference { get; }

        public PersistenceIdAlreadyAddedException(long persistenceReference)
        {
            PersistenceReference = persistenceReference;
        }
    }
}