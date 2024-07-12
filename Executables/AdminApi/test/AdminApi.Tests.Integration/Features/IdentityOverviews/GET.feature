@Integration
Feature: GET Identities

User requests an Identity Overview List

Scenario: Requesting an Identity List
	When a GET request is sent to the /Identities endpoint
	Then the response status code is 200 (OK)
	And the response contains a list of Identities
