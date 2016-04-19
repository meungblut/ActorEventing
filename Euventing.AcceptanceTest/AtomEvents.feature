Feature: AtomEventSubscription
	In order to take actions 
	As an api consumer
	I want to be told when interesting things happen

1) Notify interested parties about things happening in our domain
2) Must be interoperable
3) Must allow security (only those who are allowed to see events may see events)
4) Allow subscription through a uniform api
5) Allow criteria to be specified (e.g. tell me about payment events over £1000)
6) At least once delivery
7) Be raised as they happen

The subscriber will SUBSCRIBE to an api url and be sent a subscription URL. Once they have connected to that URL, they will be 'sent' events until they unsubscribe.

Background: Create Url
	And I have an eventing url at 'http://localhost:3600/subscriptions'

Scenario: Create an event subscription
	And I PUT a message to 'http://localhost:3600/subscriptions' with the body
	"""
	{
		"channel" : "atom",
		"subscriptionId" : "10"
	}
	"""
	Then I should receive a response with the http status code 'Accepted'

Scenario: Get event subscription details
	Given I PUT a message to 'http://localhost:3600/subscriptions' with the body
	"""
	{
		"channel" : "atom",
		"subscriptionId" : "11"
	}
	"""
	When I request the subscription from url 'http://localhost:3600/subscriptions/11'
	Then I should receive a response with the http status code 'OK'
	And a body
	"""
{"notificationChannel":{},"subscriptionId":{"id":"11"}}
	"""
	And a content type of 'application/vnd.tesco.eventSubscription+json'

Scenario: No event subscription yet
	When I request the subscription from url 'http://localhost:3600/subscriptions/63635463'
	Then I should receive a response with the http status code 'NotFound'

Scenario: Get an atom document
	Given I have subscribed to an atom feed with a subscription Id of '11111'
	And I wait for the subscription to be created at'http://localhost:3600/subscriptions/'
	When I get the feed from 'http://localhost:3600/events/atom/feed/'
	Then I should have an atom document with '0' events

Scenario: Get an atom document with events in it
	Given I have subscribed to an atom feed with a subscription Id of '222'
	And I wait for the subscription to be created at'http://localhost:3600/subscriptions/'
	When '8' events are raised within my domain
	And I get the feed from 'http://localhost:3600/events/atom/feed/'
	Then I should have an atom document with '8' events

Scenario: Create a new head document when the maximum number of events per document is breached
	Given I have subscribed to an atom feed with a subscription Id of '69857'
	And I wait for the subscription to be created at'http://localhost:3600/subscriptions/'
	When '2' more events then the maximum per document are raised within my domain
	And I get the feed from 'http://localhost:3600/events/atom/feed/'
	Then I should receive an atom document with a link to the next document in the stream from 'http://localhost:3600/events/atom/feed/'

Scenario: Retrieve documents by document id
	Given I have subscribed to an atom feed with a subscription Id of '33333'
	And I wait for the subscription to be created at'http://localhost:3600/subscriptions/'
	When '2' more events then the maximum per document are raised within my domain
	And I get the named feed from 'http://localhost:3600/events/atom/feed/33333'
	Then it should have a self reference of 'http://localhost:3600/events/atom/document/33333_1'
	Then it should have a previous reference of 'http://localhost:3600/events/atom/document/33333_0'
	Then I should be able to retrieve the earlier document by issuing a GET to its url
	And the earlier document should have a link to the new head document

Scenario: Cancel an event subscription
	Given I have subscribed to an atom feed with a subscription Id of '44444'
	And I wait for the subscription to be created at'http://localhost:3601/subscriptions/'
	And '1' events are raised within my domain
	When I cancel the subscription
	And '2' events are raised within my domain
	And I get the feed from 'http://localhost:3600/events/atom/feed/'
	Then I should have an atom document with '1' events

Scenario: Retrieve documents from a second node
	Given I have subscribed to an atom feed with a generated subscription Id
	And I wait for the subscription to be created at'http://localhost:3601/subscriptions/'
	When '2' events are raised within my domain
	And I get the feed from 'http://localhost:3601/events/atom/feed/'
	Then I should have an atom document with '2' events

Scenario: Raise events on two nodes
	Given I have subscribed to an atom feed with a subscription Id of '66666'
	And I wait for the subscription to be created at'http://localhost:3600/subscriptions/'
	When '2' events are raised within my domain
	And '2' events are raised on a different node
    And I get the feed from 'http://localhost:3600/events/atom/feed/'
	Then I should have an atom document with '4' events

Scenario: Raise events and read at the same time
	Given I have subscribed to an atom feed with a subscription Id of '987654321'
	And I wait for the subscription to be created at'http://localhost:3600/subscriptions/'
	When '10' events are raised within my domain
    And I get the feed from 'http://localhost:3600/events/atom/feed/'
	And '10' events are raised on a different node
    And I get the feed from 'http://localhost:3601/events/atom/feed/'
	And '10' events are raised within my domain
    And I get the feed from 'http://localhost:3600/events/atom/feed/'
	And '10' events are raised on a different node
    And I get the feed from 'http://localhost:3601/events/atom/feed/'
	When '10' events are raised within my domain
    And I get the feed from 'http://localhost:3600/events/atom/feed/'
	And '10' events are raised on a different node
    And I get the feed from 'http://localhost:3601/events/atom/feed/'
	And '10' events are raised within my domain
    And I get the feed from 'http://localhost:3600/events/atom/feed/'
	And '10' events are raised on a different node
    And I get the feed from 'http://localhost:3601/events/atom/feed/'

