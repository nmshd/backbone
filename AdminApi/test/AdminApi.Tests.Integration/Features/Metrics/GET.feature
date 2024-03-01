@Integration
Feature: GET Metrics

User requests an Metric List

Scenario: Requesting existing Metrics
	When a GET request is sent to the /Metrics endpoint
	Then the response status code is 200 (OK)
	And the response contains a list of Metrics
