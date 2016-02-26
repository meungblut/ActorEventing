using Euventing.Api.Startup;
using TechTalk.SpecFlow;

namespace Euventing.AcceptanceTest.Hosting
{

    [Binding]
    public class SpecflowGlobal
    {
        private static EventSystemHost host = new EventSystemHost();

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            host.Start();
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            host.Stop();
        }
    }
}
