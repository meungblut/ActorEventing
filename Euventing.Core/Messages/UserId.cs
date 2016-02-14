using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Core.Messages
{
    public class UserId
    {
        public UserId(string uuid)
        {
            Id = uuid;
        }

        public string Id { get; }
    }
}
