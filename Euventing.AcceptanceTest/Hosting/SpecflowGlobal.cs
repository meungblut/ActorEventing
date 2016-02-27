﻿using Euventing.Api.Startup;
using TechTalk.SpecFlow;

namespace Euventing.AcceptanceTest.Hosting
{

    [Binding]
    public class SpecflowGlobal
    {
        public static EventSystemHost Host = new EventSystemHost();

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