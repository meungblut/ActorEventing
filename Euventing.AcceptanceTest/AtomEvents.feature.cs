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
 testRunner.Given("I have an eventing url at \'http://localhost:3600/events\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
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
 testRunner.And("I PUT a message to \'http://localhost:3600/events\' with the body", "{\r\n\t\"channel\" : \"atom\",\r\n\t\"subscriptionId\" : \"4e347f48-fe93-4edd-9f04-0b37ed82767" +
                    "b\"\r\n}", ((TechTalk.SpecFlow.Table)(null)), "And ");
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
 testRunner.Given("I PUT a message to \'http://localhost:3600/events\' with the body", "{\r\n\t\"channel\" : \"atom\",\r\n\t\"subscriptionId\" : \"4e347f48-fe93-4edd-9f04-0b37ed82767" +
                    "c\"\r\n}", ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 39
 testRunner.When("I request the subscription from url \'http://localhost:3600/events/4e347f48-fe93-4" +
                    "edd-9f04-0b37ed82767c\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 40
 testRunner.Then("I should receive a response with the http status code \'OK\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
#line 41
 testRunner.And("a body", "{\"NotificationChannel\":{},\"SubscriptionId\":{\"Id\":\"4e347f48-fe93-4edd-9f04-0b37ed8" +
                    "2767c\"},\"AllEventMatcher\":{}}", ((TechTalk.SpecFlow.Table)(null)), "And ");
#line 45
 testRunner.And("a content type of \'application/vnd.tesco.eventSubscription+json\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("GetSimpleAtomDocument")]
        [NUnit.Framework.CategoryAttribute("atomEvents")]
        public virtual void GetSimpleAtomDocument()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("GetSimpleAtomDocument", new string[] {
                        "atomEvents"});
#line 48
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 49
 testRunner.Given("I have subscribed to an atom feed with a generated subscription Id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 50
 testRunner.When("I request the subscription from url \'http://localhost:3600/events/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 51
 testRunner.Then("I should receive a response with the http status code \'OK\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 52
 testRunner.When("\'10\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 53
 testRunner.Then("I should receive a valid atom document with \'10\' entries from \'http://localhost:3" +
                    "600/events/atom/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("GetNextDocument")]
        [NUnit.Framework.CategoryAttribute("atomEvents")]
        public virtual void GetNextDocument()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("GetNextDocument", new string[] {
                        "atomEvents"});
#line 56
this.ScenarioSetup(scenarioInfo);
#line 16
this.FeatureBackground();
#line 57
 testRunner.Given("I have subscribed to an atom feed with a generated subscription Id", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line 58
 testRunner.When("I request the subscription from url \'http://localhost:3600/events/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 59
 testRunner.Then("I should receive a response with the http status code \'OK\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line 60
 testRunner.When("\'550\' events are raised within my domain", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line 61
 testRunner.Then("I should receive an atom document with a link to the next document in the stream " +
                    "from \'http://localhost:3600/events/atom/\'", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
