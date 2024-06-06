@Integration
Feature: POST Message

User sends a Message

Scenario: Sending a Message
	Given Identities i1 and i2 with an established Relationship
	When a POST request is sent to the /Messages endpoint
	Then the response status code is 201 (Created)
	And the response contains a CreateMessageResponse

Scenario: Sending a Message to Identity to be deleted
	Given Identities i1 and i2 with an established Relationship
	And Identity i2 is to be deleted
	When a POST request is sent to the /Messages endpoint
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.message.recipientToBeDeleted"
	And the error contains a list of identities to be deleted that includes Identity i2
