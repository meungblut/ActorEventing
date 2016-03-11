using Euventing.Api.Startup;
using TechTalk.SpecFlow;

namespace Euventing.AcceptanceTest.Hosting
{

    [Binding]
    public class SpecflowGlobal
    {
        public static EventSystemHost Host = new EventSystemHost(6483, "akkaSystem", "inmem", "127.0.0.1:6483", 3600);

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            Host.Start();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            Host.Stop();
        }
    }
}
