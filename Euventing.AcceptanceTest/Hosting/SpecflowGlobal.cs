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
        public static EventSystemHost InProcessHost = new EventSystemHost(6483, "akkaSystem", "inmem", "127.0.0.1:6483", 3600);
        private static Process _outOfProcessClusterMembersHost;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            InProcessHost.Start();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            LaunchOutOfProcessHostsToJoinCluster();
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            InProcessHost.Stop();
            _outOfProcessClusterMembersHost.Kill();
        }

        private static void LaunchOutOfProcessHostsToJoinCluster()
        {
            _outOfProcessClusterMembersHost = new Process();
            _outOfProcessClusterMembersHost.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            _outOfProcessClusterMembersHost.StartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..\Euventing.ConsoleHost\bin\debug\Euventing.ConsoleHost.exe";
            _outOfProcessClusterMembersHost.StartInfo.UseShellExecute = false;
            _outOfProcessClusterMembersHost.StartInfo.Arguments = "portNumber/6484 akkaSystemName/akkaSystem seedNodes/127.0.0.1:6483 persistence/inmem";
            _outOfProcessClusterMembersHost.StartInfo.Verb = "runas";
            _outOfProcessClusterMembersHost.Start();
        }
    }
}
