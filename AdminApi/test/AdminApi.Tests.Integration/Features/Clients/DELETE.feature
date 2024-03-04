@Integration
Feature: DELETE Clients

User deletes a Client

Scenario: Deleting an existing Client
	Given a Client c
	When a DELETE request is sent to the /Clients endpoint
	Then the response status code is 204 (NO CONTENT)

Scenario: Deleting a non-existent Client
	Given a non-existent Client c
	When a DELETE request is sent to the /Clients endpoint
	Then the response status code is 404 (Not Found)
	And the response content includes an error with the error code "error.platform.recordNotFound"
