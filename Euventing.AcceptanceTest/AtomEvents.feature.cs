﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:2.0.0.0
//      SpecFlow Generator Version:2.0.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Euventing.AcceptanceTest
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "2.0.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("AtomEventSubscription")]
    public partial class AtomEventSubscriptionFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "AtomEvents.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "AtomEventSubscription", @"	In order to take actions 
	As an api consumer
	I want to be told when interesting things happen

1) Notify interested parties about things happening in our domain
2) Must be interoperable
3) Must allow per event security (only those who are allowed to see each event may see events)
4) Allow subscription through a uniform api
5) Allow criteria to be specified (e.g. tell me about payment events over £1000)
6) At least once delivery
7) Be raised as they happen
8) Require minimal supplementary infrastructure
9) Allow a shared nothing deployment
10) Minimal dependencies (utilise existing db & caching, for example)

The subscriber will SUBSCRIBE to an api url and be sent a subscription URL. Once they have connected to that URL, they will be 'sent' events until they unsubscribe.", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 19
#line 20
 testRunner.And("I have an eventing url at \'http://localhost:3600/subscriptions\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create an event subscription")]
        public virtual void CreateAnEventSubscription()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create an event subscription", ((string[])(null)));
#line 22
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line hidden
#line 23
 testRunner.And("I PUT a message to \'http://localhost:3600/subscriptions\' with the body", "{\r\n\t\"channel\" : \"atom\",\r\n\t\"subscriptionId\" : \"CreateAnEventSubscription\"\r\n}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 30
 testRunner.Then("I should receive a response with the http status code \'Accepted\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get event subscription details")]
        public virtual void GetEventSubscriptionDetails()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get event subscription details", ((string[])(null)));
#line 32
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line hidden
#line 33
 testRunner.Given("I PUT a message to \'http://localhost:3600/subscriptions\' with the body", "{\r\n\t\"channel\" : \"atom\",\r\n\t\"subscriptionId\" : \"GETEventSubscriptionDetails\"\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 40
 testRunner.When("I request the subscription from url \'http://localhost:3600/subscriptions/GETEvent" +
                    "SubscriptionDetails\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 41
 testRunner.Then("I should receive a response with the http status code \'OK\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 42
 testRunner.And("a body", "{\"notificationChannel\":{},\"subscriptionId\":{\"id\":\"GETEventSubscriptionDetails\"}}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 46
 testRunner.And("a content type of \'application/vnd.tesco.eventSubscription+json\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("No event subscription yet")]
        public virtual void NoEventSubscriptionYet()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("No event subscription yet", ((string[])(null)));
#line 48
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 49
 testRunner.When("I request the subscription from url \'http://localhost:3600/subscriptions/63635463" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 50
 testRunner.Then("I should receive a response with the http status code \'NotFound\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get an atom document")]
        public virtual void GetAnAtomDocument()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get an atom document", ((string[])(null)));
#line 52
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 53
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'GetAnAtomDocument\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 54
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 55
 testRunner.When("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 56
 testRunner.Then("I should have an atom document with \'0\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get an atom document with events in it")]
        public virtual void GetAnAtomDocumentWithEventsInIt()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get an atom document with events in it", ((string[])(null)));
#line 58
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 59
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'GetAnAtomDocumentWIt" +
                    "hEventsInIt\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 60
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 61
 testRunner.When("\'8\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 62
 testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 63
 testRunner.Then("I should have an atom document with \'8\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create a new head document when the maximum number of events per document is brea" +
            "ched")]
        public virtual void CreateANewHeadDocumentWhenTheMaximumNumberOfEventsPerDocumentIsBreached()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create a new head document when the maximum number of events per document is brea" +
                    "ched", ((string[])(null)));
#line 65
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 66
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'CreateANewHeadDocume" +
                    "nt\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 67
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 68
 testRunner.When("\'2\' more events then the maximum per document are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 69
 testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 70
 testRunner.Then("I should receive an atom document with a link to the next document in the stream " +
                    "from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieve documents by document id")]
        public virtual void RetrieveDocumentsByDocumentId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieve documents by document id", ((string[])(null)));
#line 72
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 73
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'RetrieveDocumentsByD" +
                    "ocumentId\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 74
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 75
 testRunner.When("\'2\' more events then the maximum per document are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 76
 testRunner.And("I get the named feed from \'http://localhost:3600/events/atom/feed/RetrieveDocumen" +
                    "tsByDocumentId\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 77
 testRunner.Then("it should have a self reference of \'http://localhost:3600/events/atom/document/do" +
                    "cumentId:RetrieveDocumentsByDocumentId:1\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 78
 testRunner.Then("it should have a previous reference of \'http://localhost:3600/events/atom/documen" +
                    "t/documentId:RetrieveDocumentsByDocumentId:0\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 79
 testRunner.Then("I should be able to retrieve the earlier document by issuing a GET to its url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 80
 testRunner.And("the earlier document should have a link to the new head document", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Cancel an event subscription")]
        public virtual void CancelAnEventSubscription()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Cancel an event subscription", ((string[])(null)));
#line 82
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 83
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'CancelAnEventSubscri" +
                    "ption\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 84
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 85
 testRunner.And("\'1\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 86
 testRunner.When("I cancel the subscription", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 87
 testRunner.And("\'2\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 88
 testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 89
 testRunner.Then("I should have an atom document with \'1\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieve documents from a second node")]
        [NUnit.Framework.IgnoreAttribute("Ignored scenario")]
        public virtual void RetrieveDocumentsFromASecondNode()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieve documents from a second node", new string[] {
                        "ignore"});
#line 92
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 93
 testRunner.Given("I have subscribed to an atom feed with a generated subscription Id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 94
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3601/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 95
 testRunner.When("\'2\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 96
 testRunner.And("I get the feed from \'http://localhost:3601/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 97
 testRunner.Then("I should have an atom document with \'2\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Raise events on two nodes")]
        [NUnit.Framework.IgnoreAttribute("Ignored scenario")]
        public virtual void RaiseEventsOnTwoNodes()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Raise events on two nodes", new string[] {
                        "ignore"});
#line 100
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 101
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'66666\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 102
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 103
 testRunner.When("\'2\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 104
 testRunner.And("\'2\' events are raised on a different node", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 105
    testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 106
 testRunner.Then("I should have an atom document with \'4\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Raise events and read at the same time")]
        [NUnit.Framework.IgnoreAttribute("Ignored scenario")]
        public virtual void RaiseEventsAndReadAtTheSameTime()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Raise events and read at the same time", new string[] {
                        "ignore"});
#line 109
this.ScenarioSetup(scenarioInfo);
#line 19
this.FeatureBackground();
#line 110
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'987654321\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 111
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 112
 testRunner.When("\'10000\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 113
    testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 114
 testRunner.And("\'10000\' events are raised on a different node", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 115
    testRunner.And("I get the feed from \'http://localhost:3601/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 116
 testRunner.And("\'10000\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 117
    testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 118
 testRunner.And("\'10000\' events are raised on a different node", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 119
    testRunner.And("I get the feed from \'http://localhost:3601/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 120
 testRunner.When("\'10000\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 121
    testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 122
 testRunner.And("\'10000\' events are raised on a different node", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 123
    testRunner.And("I get the feed from \'http://localhost:3601/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 124
 testRunner.And("\'10000\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 125
    testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 126
 testRunner.And("\'10000\' events are raised on a different node", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 127
    testRunner.And("I get the feed from \'http://localhost:3601/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
