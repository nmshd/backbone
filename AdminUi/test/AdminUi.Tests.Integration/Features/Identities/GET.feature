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
