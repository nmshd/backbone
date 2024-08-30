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

Scenario: Requesting a list of Tokens contains tokens with forIdentity which were created by me
	Given Identities i1 and i2
	And a Token t created by i1 where forIdentity is the address of i2
	When i1 sends a GET request to the /Tokens endpoint and passes t.id
	Then the response status code is 200 (Ok)
	And the response contains t

Scenario: Requesting a list of Tokens contains tokens with forIdentity which were created for me
	Given Identities i1 and i2 
	And a Token t created by i1 where forIdentity is the address of i2 
	When i2 sends a GET request to the /Tokens endpoint and passes t.id
	Then the response status code is 200 (Ok) 
	And the response contains t

Scenario: Requesting a list of Tokens contains tokens which can be collected by me
	Given Identities i1 and i2
	And a Token t created by i1 where forIdentity is the address of i2 
	When i1 sends a GET request to the /Tokens endpoint
	Then the response status code is 200 (Ok)
	And the response contains t

Scenario: Requesting a list of Tokens does not contain tokens with forIdentity which were created for someone else
	Given Identities i1, i2 and i3
	And a Token t created by i1 where forIdentity is the address of i2
	When i3 sends a GET request to the /Tokens endpoint and passes t.id
	Then the response does not contain t
