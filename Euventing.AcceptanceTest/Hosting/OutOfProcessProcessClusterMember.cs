using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Eventing.AcceptanceTest.Hosting
{
    internal class OutOfProcessProcessClusterMember
    {
        private Process process;
        private readonly int eventsPerDocument;

        public OutOfProcessProcessClusterMember(int eventsPerDocument)
        {
            this.eventsPerDocument = eventsPerDocument;
        }

        internal void Start()
        {
            process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.StartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\..\..\..\Euventing.ConsoleHost\bin\debug\Eventing.ConsoleHost.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = "portNumber/6484 akkaSystemName/akkaSystem seedNodes/127.0.0.1:6483 persistence/local entriesPerDocument/" + eventsPerDocument;
            process.StartInfo.Verb = "runas";
            process.Start();
        }

        internal void Stop()
        {
            process.Kill();
        }
    }
}
