@Integration
Feature: GET Tier Details

User requests a Tier's Details

Scenario: Requesting a Tier by Id
	Given a Tier t
	When a GET request is sent to the /Tiers/{t.id} endpoint
	Then the response status code is 200 (OK)
	And the response contains Tier t