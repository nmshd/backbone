@Integration
Feature: POST Tier

Administrator Creates a Tier

Scenario: Creating a Tier
	When a POST request is sent to the /Tiers endpoint
	Then the response status code is 201 (Created)
	And the response contains a Tier

Scenario: Creating a Tier with a name that already exists
	Given a Tier t
	When a POST request is sent to the /Tiers endpoint with the name t.Name
	Then the response status code is 400 (Bad Request)
	And the response content includes an error with the error code "error.platform.validation.device.tierNameAlreadyExists"
