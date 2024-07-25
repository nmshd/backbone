@Integration
Feature: UPDATE Device

User updates a Device

Scenario: Updating own Device with valid data
	Given an Identity i with a device d
	When i sends a PUT request to the Devices/Self endpoint with the communication language 'de'
	Then the response status code is 204 (No Content)
	And the device on the Backbone of i has the new communication language

Scenario: Updating own Device with invalid data
	Given an Identity i with a device d
	When i sends a PUT request to the Devices/Self endpoint with a non-existent language code
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.invalidDeviceCommunicationLanguage"

Scenario: Updating own Device password with valid data
	Given an Identity i with a device d
	When i sends a PUT request to the Devices/Self/Password endpoint with the new password 'password'
	Then the response status code is 204 (No Content)
