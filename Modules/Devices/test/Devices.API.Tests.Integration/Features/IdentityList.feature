@Integration
Feature: Get Identity List

User requests an Identity List

Scenario: Requesting an Identity List as an authenticated user
	When a GET request is sent to the Identities/ endpoint
	Then the response status code is 200 (OK)
	And the response contains a list
