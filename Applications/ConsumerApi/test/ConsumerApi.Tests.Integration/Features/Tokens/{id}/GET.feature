@Integration
Feature: GET Token

User requests a Token

	Scenario: Requesting an own Token as an authenticated user
		Given Identity i and Token t
		When i sends a GET request to the Tokens/{id} endpoint with t.Id
		Then the response status code is 200 (OK)
		And the response contains a Token

	Scenario: Requesting an own Token as an anonymous user
		Given the user is unauthenticated
		And Identity i and Token t
		When a GET request is sent to the Tokens/{id} endpoint with t.Id
		Then the response status code is 200 (OK)
		And the response contains a Token

	Scenario: Requesting a peer Token as an authenticated user
		Given Identity i1
		And Identity i2 and Token t2
		When i1 sends a GET request to the Tokens/{id} endpoint with t2.Id
		Then the response status code is 200 (OK)
		And the response contains a Token

	Scenario: Requesting a nonexistent Token as an authenticated user
		Given Identity i1
		When i1 sends a GET request to the Tokens/{id} endpoint with "TOKthisisnonexisting"
		Then the response status code is 404 (Not Found)
		And the response content contains an error with the error code "error.platform.recordNotFound"
