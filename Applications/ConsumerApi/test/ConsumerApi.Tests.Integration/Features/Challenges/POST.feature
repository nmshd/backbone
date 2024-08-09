@Integration
Feature: POST Challenge

User creates a Challenge

	Scenario: Creating a Challenge as an anonymous user
		Given the user is unauthenticated
		When a POST request is sent to the /Challenges endpoint
		Then the response status code is 201 (Created)
		And the response contains a Challenge
		And the Challenge has a valid expiration date
		And the Challenge does not contain information about the creator

	Scenario: Creating a Challenge as an authenticated user
		Given Identity i
		When i sends a POST request to the /Challenges endpoint
		Then the response status code is 201 (Created)
		And the response contains a Challenge
		And the Challenge has a valid expiration date
		And the Challenge contains information about the creator
