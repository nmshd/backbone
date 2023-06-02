@Integration
Feature: GET Clients

User requests an Client List

Scenario: Requesting an Client List
	When a GET request is sent to the Clients/ endpoint
	Then the response status code is 200 (OK)
	And the response contains a paginated list of Clients
