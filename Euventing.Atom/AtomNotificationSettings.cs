using Euventing.Atom.ShardSupport.Document;
using Euventing.Core.Notifications;

namespace Euventing.Atom
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
