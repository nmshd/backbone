@Integration
Feature: UPDATE Device

User updates a Device

Scenario: Updating own Device with valid data
	Given an Identity i with a device d1
	When a PUT request is sent to the Devices/Self endpoint with a valid payload
	Then the response status code is 204 (No Content)

Scenario: Updating own Device with invalid data
	Given an Identity i with a device d1
	When a PUT request is sent to the Devices/Self endpoint with an invalid payload
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.invalidPropertyValue"
