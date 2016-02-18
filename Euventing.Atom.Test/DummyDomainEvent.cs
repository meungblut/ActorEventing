using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Euventing.Core.Messages;

namespace Euventing.Atom.Test
{
    public class DummyDomainEvent : DomainEvent
    {
        public DummyDomainEvent(string id) : base(id)
        {
        }
    }
}
