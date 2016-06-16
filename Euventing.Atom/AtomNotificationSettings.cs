using Eventing.Atom.Document.Actors.ShardSupport.Document;
using Eventing.Core.Notifications;

namespace Eventing.Atom
{
    public class AtomNotificationSettings : NotificationSettings
    {
        public AtomNotificationSettings(ShardedAtomFeedFactory atomFeedFactory)
        {
            AtomEventNotifier notifier = new AtomEventNotifier(atomFeedFactory);
            base.AddNotifierType(typeof(AtomNotificationChannel), notifier);
        }
    }
}
