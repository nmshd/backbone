@Integration
Feature: POST Identities/{id}/DeletionProcess

Support starts a deletion process

Scenario: Starting a deletion process as support
	Given an Identity i
	When a POST request is sent to the /Identities/{i.id}/DeletionProcesses endpoint
	Then the response status code is 201 (Created)
	And the response contains a Deletion Process

Scenario: There can only be one active deletion process
	Given an Identity i
	And an active deletion process for Identity i exists
	When a POST request is sent to the /Identities/{i.id}/DeletionProcesses endpoint
	Then the response status code is 400 (Bad Request)
	And the response content includes an error with the error code "error.platform.validation.device.onlyOneActiveDeletionProcessAllowed"
