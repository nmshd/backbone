@Integration
Feature: POST Log

UI Creates a Log

Scenario: Creating a Log
	When a POST request is sent to the /Logs endpoint
	Then the response status code is 204 (No Content)

Scenario: Creating a Log with an invalid Log Level fails
	When a POST request is sent to the /Logs endpoint with an invalid Log Level
	Then the response status code is 400 (Bad Request)
	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
