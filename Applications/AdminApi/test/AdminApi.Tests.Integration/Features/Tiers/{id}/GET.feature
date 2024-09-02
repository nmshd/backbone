@Integration
Feature: GET Tiers/{id}

User requests a Tier

Scenario: Requesting a Tier by id
	Given a Tier t
	When a GET request is sent to the /Tiers/{t.id} endpoint
	Then the response status code is 200 (OK)
	And the response contains Tier t

Scenario: Requesting a Tier with non existent address
	When a GET request is sent to the /Tiers/{nonExistentTierId} endpoint
	Then the response status code is 404 (Not Found)
	And the response content contains an error with the error code "error.platform.recordNotFound"
