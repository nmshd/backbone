@Integration
Feature: Get Identity List

User requests an Identity List

Scenario: Requesting an Identity List as an authenticated user
	Given the user is authenticated
	When a GET request is sent to the Identities/ endpoint
	Then the response status code is 200 (OK)
	And the response contains a list

# TODO remove the comment char and the last line once authentication is enabled.
# For now, authentication is disabled due to the Admin UI implementation.

Scenario: Requesting an Identity List as a non-authenticated user
	Given the user is not authenticated
	When a GET request is sent to the Identities/ endpoint
	#Then the response status code is 401 (Unauthorized)
	Then the response status code is 200 (Ok)