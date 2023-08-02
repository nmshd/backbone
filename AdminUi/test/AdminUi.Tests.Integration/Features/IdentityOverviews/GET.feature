@Integration
Feature: GET IdentityOverviews

User requests an IdentityOverview List

Scenario: Requesting an Identity List
	When a GET request is sent to the /Identities/Overview endpoint
	Then the response status code is 200 (OK)
	And the response contains a paginated list of Identity Overviews
