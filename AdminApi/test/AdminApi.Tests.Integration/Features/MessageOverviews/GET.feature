@Integration
Feature: GET Messages

User requests a Message Overview List

Scenario: Requesting an Incoming Message List of an Identity
	Given an Identity i
	When a GET request is sent to the /Messages endpoint with type 'Incoming' and participant i.Address
	Then the response status code is 200 (OK)
	And the response contains a paginated list of Messages

Scenario: Requesting an Outgoing Message List of an Identity
	Given an Identity i
	When a GET request is sent to the /Messages endpoint with type 'Outgoing' and participant i.Address
	Then the response status code is 200 (OK)
	And the response contains a paginated list of Messages

Scenario: Requesting an invalid type Message List of an Identity
	Given an Identity i
	When a GET request is sent to the /Messages endpoint with type 'InvalidType' and participant i.Address
	Then the response status code is 400 (Bad request)
	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
