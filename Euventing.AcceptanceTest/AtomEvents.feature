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
	Given I have an eventing url at 'http://localhost:3600/events'

@subscription
Scenario: Create an event subscription
	And I PUT a message to 'http://localhost:3600/events' with the body
	"""
	{
		"channel" : "atom",
		"subscriptionId" : "4e347f48-fe93-4edd-9f04-0b37ed82767b"
	}
	"""
	Then I should receive a response with the http status code 'Accepted'

@subscription
Scenario: Get event subscription details
	Given I PUT a message to 'http://localhost:3600/events' with the body
	"""
	{
		"channel" : "atom",
		"subscriptionId" : "4e347f48-fe93-4edd-9f04-0b37ed82767c"
	}
	"""
	When I request the subscription from url 'http://localhost:3600/events/4e347f48-fe93-4edd-9f04-0b37ed82767c'
	Then I should receive a response with the http status code 'OK'
	And a body
	"""
{"NotificationChannel":{},"SubscriptionId":{"Id":"4e347f48-fe93-4edd-9f04-0b37ed82767c"},"AllEventMatcher":{}}
	"""
	And a content type of 'application/vnd.tesco.eventSubscription+json'

@atomEvents
Scenario: Get an atom document with events in it
	Given I have subscribed to an atom feed with a generated subscription Id
	When I request the subscription from url 'http://localhost:3600/events/'
	Then I should receive a response with the http status code 'OK'
	When '10' events are raised within my domain
	Then I should receive a valid atom document with '10' entries from 'http://localhost:3600/events/atom/'

@atomEvents
Scenario: Create a new head document when the maximum number of events per document is breached
	Given I have subscribed to an atom feed with a generated subscription Id
	When I request the subscription from url 'http://localhost:3600/events/'
	Then I should receive a response with the http status code 'OK'
	When '152' events are raised within my domain
	Then I should receive an atom document with a link to the next document in the stream from 'http://localhost:3600/events/atom/'

	@atomEvents
Scenario: Retrieve documents by document id rather than head document id 
	Given I have subscribed to an atom feed with a generated subscription Id
	When I request the subscription from url 'http://localhost:3600/events/'
	Then I should receive a response with the http status code 'OK'
	When '152' events are raised within my domain
	Then I should receive an atom document with a link to the next document in the stream from 'http://localhost:3600/events/atom/'
	Then I should be able to retrieve the earlier document by issuing a GET to its url