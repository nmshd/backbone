@Integration
Feature: Get Challenge By Id

User requests a Challenge

Scenario: Requesting a Challenge as an authenticated user
	Given the user is authenticated
	And a Challenge c
	When a GET request is sent to the Challenges/{id} endpoint with "c.Id"
	Then the response status code is 200
	And the response contains a Challenge

Scenario: Requesting a Challenge as an anonymous user
	Given the user is unauthenticated
	And a Challenge c
	When a GET request is sent to the Challenges/{id} endpoint with "c.Id"
	Then the response status code is 401

Scenario: Requesting a nonexistent Challenge as an authenticated user
	Given the user is authenticated
	When a GET request is sent to the Challenges/{id} endpoint with "CHLthisisnonexisting"
	Then the response status code is 404

Scenario: Requesting a Challenge with an invalid id as an authenticated user
	Given the user is authenticated
	When a GET request is sent to the Challenges/{id} endpoint with "a123"
	Then the response status code is 400
	And the response content includes an error with the error code "error.platform.invalidId"

Scenario: Requesting a Challenge with an unsupported Accept Header as an authenticated user
	Given the user is authenticated
	And the Accept header is 'application/xml'
	When a GET request is sent to the Challenges/{id} endpoint with a valid Id
	Then the response status code is 406