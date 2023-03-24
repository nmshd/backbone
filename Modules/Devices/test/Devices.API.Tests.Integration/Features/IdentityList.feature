@Integration
Feature: Get Identity List

User requests an Identity List

# TODO uncomment the Given below and remove the Ignore tag once authentication is enabled.
# For now, authentication is disabled due to the Admin UI implementation.

Scenario: Requesting an Identity List as an authenticated user
	#Given the user is authenticated
	When a GET request is sent to the Identities/ endpoint
	Then the response status code is 200 (OK)
	And the response contains a list

@Ignore
Scenario: Requesting an Identity List as a non-authenticated user
	Given the user is not authenticated
	When a GET request is sent to the Identities/ endpoint
	Then the response status code is 401 (Unauthorized)