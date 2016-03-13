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
        public static EventSystemHost InProcessHost = new EventSystemHost(6483, "akkaSystem", "sqlite", "127.0.0.1:6483", 3600);
        private static Process _outOfProcessShardMembersHost;

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
            _outOfProcessShardMembersHost.Kill();
        }

        private static void LaunchOutOfProcessHostsToJoinCluster()
        {
            _outOfProcessShardMembersHost = new Process();
            _outOfProcessShardMembersHost.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            _outOfProcessShardMembersHost.StartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..\Euventing.ConsoleHost\bin\debug\Euventing.ConsoleHost.exe";
            _outOfProcessShardMembersHost.StartInfo.UseShellExecute = false;
            _outOfProcessShardMembersHost.StartInfo.Arguments = "portNumber/6484 akkaSystemName/akkaSystem seedNodes/127.0.0.1:6483 persistence/sqlite";
            _outOfProcessShardMembersHost.StartInfo.Verb = "runas";
            _outOfProcessShardMembersHost.Start();
        }
    }
}
