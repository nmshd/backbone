@Integration
Feature: GET Identities

User requests an Identity List

Scenario: Requesting an Identity List
	When a GET request is sent to the /Identities endpoint
	Then the response status code is 200 (OK)
	And the response contains a paginated list of Identities

Scenario: Requesting an Identity
	Given an Identity i
	When a GET request is sent to the /Identities/{i.address} endpoint
	Then the response status code is 200 (OK)
	And the response contains Identity i

Scenario: Requesting an inexistent Identity
	When a GET request is sent to the /Identities/{address} endpoint with an inexistent address
	Then the response status code is 404 (NOTFOUND)
	And the response content includes an error with the error code "error.platform.recordNotFound"
