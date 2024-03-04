@Integration
Feature: GET Tokens

User requests multiple Tokens

Scenario: Requesting a list of own Tokens
	Given the user is authenticated
	And the user created multiple Tokens
	When a GET request is sent to the Tokens endpoint with a list of ids of own Tokens
	Then the response status code is 200 (OK)
	And the response contains all Tokens with the given ids

Scenario: Requesting an own and peer Token
	Given the user is authenticated
	And an own Token t
	And a peer Token p
	When a GET request is sent to the Tokens endpoint with a list containing t.Id, p.Id
	Then the response status code is 200 (OK)
	And the response contains both Tokens
