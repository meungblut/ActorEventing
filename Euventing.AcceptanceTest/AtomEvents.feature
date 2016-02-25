Feature: AtomEventSubscription
	In order to take actions 
	As an api consumer
	I want to be told when interesting things happen

1) Notify interested parties about things happening in our domain
2) Must be interoperable
3) Must allow security (only those who are allowed to see events may see events)
4) Allow subscription through a uniform api
5) Allow criteria to be specified (e.g. tell me about payments events over £1000)
6) At least once delivery
7) Be raised as they happen

The subscriber will SUBSCRIBE to an api url and be sent a subscription URL. Once they have connected to that URL, they will be 'sent' events until they unsubscribe.

@ignore
@subscription
Scenario: Subscribe to all events
	And I PUT a message to 'http://localhost:3605/events' with the body
	"""
	{
		"channel" : "atom",
		"subscriptionId" : "4e347f48-fe93-4edd-9f04-0b37ed82767b"
	}
	"""
	Then I should receive a response with the http status code 'OK'
	And a content type of 'application/vnd.tesco.eventSubscription+json'
	And a body
	"""
{"Channel":"Atom","Id":"4e347f48-fe93-4edd-9f04-0b37ed82767b","EventResourceLocation":"http://localhost:3605/atom/4e347f48-fe93-4edd-9f04-0b37ed82767b"}
	"""
@ignore
@atomEvents
Scenario: GetSimpleAtomDocument
	Given I have subscribed to an atom feed with a subscription Id of '4e347f48-fe93-4edd-9f04-0b37ed82767d'
	When '10' events are raised within my domain
	Then I should receive a valid atom document with '10' entries

@ignore
@atomEvents
Scenario: GetNextDocument
	Given I have subscribed to an atom feed with a subscription Id of '4e347f48-fe93-4edd-9f04-0b37ed82767c'
	When '550' events are raised within my domain
	Then I should receive an atom document with a link to the next document in the stream