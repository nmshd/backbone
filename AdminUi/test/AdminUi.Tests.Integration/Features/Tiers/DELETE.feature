@Integration
Feature: DELETE Tier

User deletes a Tier

Scenario: Deleting a Tier
	Given a Tier t
	When a DELETE request is sent to the /Tiers/{t.Id} endpoint
	Then the response status code is 200 (OK)
