﻿Feature: POST Identities/Self/DeletionProcess

User starts a deletion process

Scenario: Starting a deletion process
	Given no active deletion process for the user exists
	When a POST request is sent to the /Identities/Self/DeletionProcess endpoint
	Then the response status code is 201 (Created)

Scenario: There can only be one active deletion process
	Given an active deletion process for the user exists
	When a POST request is sent to the /Identities/Self/DeletionProcess endpoint
	Then the response status code is 400 (Bad Request)
	And the response content includes an error with the error code "error.platform.validation.device.onlyOneActiveDeletionProcessAllowed"
