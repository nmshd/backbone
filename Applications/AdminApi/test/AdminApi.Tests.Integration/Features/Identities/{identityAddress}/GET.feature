@Integration
Feature: GET Identities/{identityAddress}

User requests an Identity

Scenario: Requesting an Identity by address
	Given an Identity i
	When a GET request is sent to the /Identities/{i.address} endpoint
	Then the response status code is 200 (OK)
	And the response contains Identity i
