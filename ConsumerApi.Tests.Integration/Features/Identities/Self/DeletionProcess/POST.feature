@Integration
Feature: POST Identities/Self/DeletionProcess

User starts a deletion process

Scenario: Starting a deletion process
    Given Identity i
 	And no active deletion process for the identity exists
 	When i sends a POST request to the /Identities/Self/DeletionProcesses endpoint
 	Then the response status code is 201 (Created)
 	And the response contains a Deletion Process

Scenario: There can only be one active deletion process
    Given Identity i
 	And an active deletion process for i exists
 	When i sends a POST request to the /Identities/Self/DeletionProcesses endpoint
 	Then the response status code is 400 (Bad Request)
 	And the response content contains an error with the error code "error.platform.validation.device.onlyOneActiveDeletionProcessAllowed"
