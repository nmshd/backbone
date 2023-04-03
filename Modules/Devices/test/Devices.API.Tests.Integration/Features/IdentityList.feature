@Integration
Feature: Get Identity List

User requests an Identity List

Scenario: Requesting an Identity List
	When a GET request is sent to the Identities/ endpoint
	Then the response status code is 200 (OK)
	And the response contains a paginated list of Identities
