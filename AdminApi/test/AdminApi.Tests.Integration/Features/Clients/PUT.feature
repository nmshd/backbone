@Integration
Feature: PUT Clients

User updates a Client

Scenario: Update a Client
	Given a Client c
	When a PUT request is sent to the /Clients/{c.ClientId} endpoint
	Then the response status code is 200 (OK)
	And the response contains Client c
	And the Client in the Backend was successfully updated

Scenario: Removing the max identities of an existing Client
	Given a Client c
	When a PUT request is sent to the /Clients/{c.ClientId} endpoint with a null value for maxIdentities
	Then the response status code is 200 (OK)
	And the response contains Client c
	And the Client in the Backend has a null value for maxIdentities

Scenario: Changing the default tier of an existing Client with a non-existent tier id
	Given a Client c
	When a PUT request is sent to the /Clients/{c.ClientId} endpoint with a non-existent tier id
	Then the response status code is 400 (Bad request)
	And the response content includes an error with the error code "error.platform.validation.device.tierIdInvalidOrDoesNotExist"

Scenario: Changing the default tier of a non-existing Client
	When a PUT request is sent to the /Clients/{c.clientId} endpoint with a non-existing clientId
	Then the response status code is 404 (Not Found)
	And the response content includes an error with the error code "error.platform.recordNotFound"
