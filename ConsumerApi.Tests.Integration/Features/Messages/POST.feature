@Integration
Feature: POST Message

User sends a Message

Scenario: Sending a Message
	Given Identities i1 and i2 with an established Relationship
	When i1 sends a POST request to the Messages endpoint with i2 as recipient
	Then the response status code is 201 (Created)
	And the response contains a SendMessageResponse

Scenario: Sending a Message to Identity to be deleted
	Given Identities i1 and i2 with an established Relationship
	And i2 is in status "ToBeDeleted"
	When i1 sends a POST request to the Messages endpoint with i2 as recipient
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.message.recipientToBeDeleted"
	And the response error contains a list of Identities to be deleted that includes i2
