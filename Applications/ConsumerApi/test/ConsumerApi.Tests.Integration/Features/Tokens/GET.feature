@Integration
Feature: GET Tokens

User requests multiple Tokens

	Scenario: Requesting a list of own Tokens
		Given Identity i
		And i created multiple Tokens
		When i sends a GET request to the Tokens endpoint with a list of ids of own Tokens
		Then the response status code is 200 (OK)
		And the response contains all Tokens created by i with the given ids

	Scenario: Requesting an own and peer Token
		Given Identity i1 and Token t1
		And Identity i2 and Token t2
		When i1 sends a GET request to the Tokens endpoint with a list containing t1.Id, t2.Id
		Then the response status code is 200 (OK)
		And the response contains t1 and t2
