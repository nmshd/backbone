@Integration
Feature: GET Clients

User requests a Client Overview List

Scenario: Requesting a list of existing Clients
	When a GET request is sent to the /Clients endpoint
	Then the response status code is 200 (OK)
	And the response contains a paginated list of Clients
