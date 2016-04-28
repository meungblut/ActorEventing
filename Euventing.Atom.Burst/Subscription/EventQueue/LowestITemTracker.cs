using System.Collections.Concurrent;
using System.Linq;

namespace Euventing.Atom.Burst.Subscription.EventQueue
{
    public class LowestItemTracker
    {
        readonly ConcurrentDictionary<string, long> data = new ConcurrentDictionary<string, long>();

        public void AddEntry(long entry, string trackerId)
        {
            data.AddOrUpdate(trackerId, entry, (l, s) => entry);
        }

        public long GetLowestValue()
        {
            return data.Min(x => x.Value);
        }
    }
}
