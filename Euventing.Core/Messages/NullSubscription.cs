﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Euventing.Core.Messages
{
    public class NullSubscription : SubscriptionMessage
    {
        public NullSubscription() : base(null, null, null, null)
        {
        }
    }
}