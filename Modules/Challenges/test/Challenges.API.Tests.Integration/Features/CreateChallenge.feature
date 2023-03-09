@Integration
Feature: Create a Challenge

User creates a Challenge

Scenario: Creating a Challenge as an anonymous user
	Given the user is unauthenticated
	When a POST request is sent to the Challenges endpoint
	Then the response status code is 201
	And the response contains a Challenge

Scenario: Creating a Challenge as an authenticated user
	Given the user is authenticated
	When a POST request is sent to the Challenges endpoint
	Then the response status code is 201
	And the response contains a Challenge

Scenario: Creating a Challenge with a JSON sent in the request content
	When a POST request is sent to the Challenges endpoint with
		| Key  | Value                              |
		| Body | {"this": "is some arbitrary json"} |
	Then the response status code is 201
	And the response contains a Challenge

Scenario: Creating a Challenge with an invalid JSON sent in the request content
	When a POST request is sent to the Challenges endpoint with
		| Key  | Value                             |
		| Body | { "thisJSON": "has an extra }" }} |
	Then the response status code is 400

Scenario: Creating a Challenge with an unsupported Content-Type header
	When a POST request is sent to the Challenges endpoint with
		| Key         | Value                              |
		| ContentType | application/xml                    |
		| Body        | <this>is some arbitrary xml</this> |
	Then the response status code is 415