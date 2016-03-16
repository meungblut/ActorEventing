using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Euventing.Api.Startup;
using TechTalk.SpecFlow;
using Euventing.ConsoleHost;

namespace Euventing.AcceptanceTest.Hosting
{

    [Binding]
    public class SpecflowGlobal
    {
        //public static EventSystemHost InProcessHost = new EventSystemHost(6483, "akkaSystem", "inmem", "127.0.0.1:6483", 3600, 10);
        //private static OutOfProcessProcessClusterMember _outOfProcessClusterMembersHost;

        //[BeforeTestRun]
        //public static void BeforeTestRun()
        //{
        //    InProcessHost.Start();
        //    Thread.Sleep(TimeSpan.FromSeconds(1));
        //    _outOfProcessClusterMembersHost = new OutOfProcessProcessClusterMember(10);
        //    _outOfProcessClusterMembersHost.Start();
        //    Thread.Sleep(TimeSpan.FromSeconds(1));
        //}

        //[AfterTestRun]
        //public static void AfterTestRun()
        //{
        //    InProcessHost.Stop();
        //    _outOfProcessClusterMembersHost.Stop();
        //}

    }
}
