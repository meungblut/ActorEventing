using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Core.Messages
{
    public abstract class DomainEvent
    {
        public string Id { get; private set; }
        public DateTime OccurredTime { get; private set; }

        protected DomainEvent()
        {
            
        }
        protected DomainEvent(string id, DateTime occurred)
        {
            Id = id;
            OccurredTime = occurred;
        }
    }
}
