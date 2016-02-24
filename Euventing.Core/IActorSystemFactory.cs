﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;

namespace Euventing.Core
{
    public interface IActorSystemFactory
    {
        ActorSystem GetActorSystem();
    }
}
