@Integration
Feature: GET Identities/{identityAddress}

User requests an Identity

Scenario: Requesting an Identity by address
	Given an Identity i
	When a GET request is sent to the /Identities/{i.address} endpoint
	Then the response status code is 200 (OK)
	And the response contains Identity i

Scenario: Requesting an Identity with non existent address
	When a GET request is sent to the /Identities/{address} endpoint with an inexistent address
	Then the response status code is 404 (Not Found)
	And the response content contains an error with the error code "error.platform.recordNotFound"
