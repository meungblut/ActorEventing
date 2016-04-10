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
3) Must allow security (only those who are allowed to see events may see events)
4) Allow subscription through a uniform api
5) Allow criteria to be specified (e.g. tell me about payment events over £1000)
6) At least once delivery
7) Be raised as they happen

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
#line 16
#line 17
 testRunner.And("I have an eventing url at \'http://localhost:3600/subscriptions\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create an event subscription")]
        [NUnit.Framework.CategoryAttribute("subscription")]
        public virtual void CreateAnEventSubscription()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create an event subscription", new string[] {
                        "subscription"});
#line 20
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line hidden
#line 21
 testRunner.And("I PUT a message to \'http://localhost:3600/subscriptions\' with the body", "{\r\n\t\"channel\" : \"atom\",\r\n\t\"subscriptionId\" : \"10\"\r\n}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 28
 testRunner.Then("I should receive a response with the http status code \'Accepted\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get event subscription details")]
        [NUnit.Framework.CategoryAttribute("subscription")]
        public virtual void GetEventSubscriptionDetails()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get event subscription details", new string[] {
                        "subscription"});
#line 31
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line hidden
#line 32
 testRunner.Given("I PUT a message to \'http://localhost:3600/subscriptions\' with the body", "{\r\n\t\"channel\" : \"atom\",\r\n\t\"subscriptionId\" : \"11\"\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 39
 testRunner.When("I request the subscription from url \'http://localhost:3600/subscriptions/11\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 40
 testRunner.Then("I should receive a response with the http status code \'OK\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 41
 testRunner.And("a body", "{\"notificationChannel\":{},\"subscriptionId\":{\"id\":\"11\"}}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 45
 testRunner.And("a content type of \'application/vnd.tesco.eventSubscription+json\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("No event subscription yet")]
        [NUnit.Framework.CategoryAttribute("subscription")]
        public virtual void NoEventSubscriptionYet()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("No event subscription yet", new string[] {
                        "subscription"});
#line 48
this.ScenarioSetup(scenarioInfo);
#line 16
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
        [NUnit.Framework.CategoryAttribute("atomEvents")]
        public virtual void GetAnAtomDocument()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get an atom document", new string[] {
                        "atomEvents"});
#line 53
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 54
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'11111\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 55
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 56
 testRunner.When("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 57
 testRunner.Then("I should have an atom document with \'0\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Get an atom document with events in it")]
        [NUnit.Framework.CategoryAttribute("atomEvents")]
        public virtual void GetAnAtomDocumentWithEventsInIt()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Get an atom document with events in it", new string[] {
                        "atomEvents"});
#line 60
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 61
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'222\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 62
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 63
 testRunner.When("\'8\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 64
 testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 65
 testRunner.Then("I should have an atom document with \'8\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Create a new head document when the maximum number of events per document is brea" +
            "ched")]
        [NUnit.Framework.CategoryAttribute("atomEvents")]
        public virtual void CreateANewHeadDocumentWhenTheMaximumNumberOfEventsPerDocumentIsBreached()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Create a new head document when the maximum number of events per document is brea" +
                    "ched", new string[] {
                        "atomEvents"});
#line 68
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 69
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'69857\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 70
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 71
 testRunner.When("\'12\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 72
 testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 73
 testRunner.Then("I should receive an atom document with a link to the next document in the stream " +
                    "from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieve documents by document id")]
        [NUnit.Framework.CategoryAttribute("atomEvents")]
        public virtual void RetrieveDocumentsByDocumentId()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieve documents by document id", new string[] {
                        "atomEvents"});
#line 76
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 77
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'33333\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 78
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 79
 testRunner.When("\'52\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 80
 testRunner.Then("I should receive an atom document with a link to the next document in the stream " +
                    "from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 81
 testRunner.Then("I should be able to retrieve the earlier document by issuing a GET to its url", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 82
 testRunner.And("the earlier document should have a link to the new head document", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Cancel an event subscription")]
        [NUnit.Framework.CategoryAttribute("atomEvents")]
        public virtual void CancelAnEventSubscription()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Cancel an event subscription", new string[] {
                        "atomEvents"});
#line 85
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 86
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'44444\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 87
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3601/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 88
 testRunner.And("\'1\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 89
 testRunner.When("I cancel the subscription", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 90
 testRunner.And("\'2\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 91
 testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 92
 testRunner.Then("I should have an atom document with \'1\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Retrieve documents from a second node")]
        [NUnit.Framework.CategoryAttribute("multinode")]
        public virtual void RetrieveDocumentsFromASecondNode()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Retrieve documents from a second node", new string[] {
                        "multinode"});
#line 95
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 96
 testRunner.Given("I have subscribed to an atom feed with a generated subscription Id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 97
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3601/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 98
 testRunner.When("\'2\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 99
 testRunner.And("I get the feed from \'http://localhost:3601/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 100
 testRunner.Then("I should have an atom document with \'2\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Raise events on two nodes")]
        [NUnit.Framework.CategoryAttribute("multinode")]
        public virtual void RaiseEventsOnTwoNodes()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Raise events on two nodes", new string[] {
                        "multinode"});
#line 103
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 104
 testRunner.Given("I have subscribed to an atom feed with a subscription Id of \'66666\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 105
 testRunner.And("I wait for the subscription to be created at\'http://localhost:3600/subscriptions/" +
                    "\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 106
 testRunner.When("\'2\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 107
 testRunner.And("\'2\' events are raised on a different node", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 108
    testRunner.And("I get the feed from \'http://localhost:3600/events/atom/feed/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 109
 testRunner.Then("I should have an atom document with \'4\' events", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
