@Integration
Feature: POST Identities/Self/DeletionProcesses

User starts a Identity Deletion Process

    Scenario: Starting an Identity Deletion Process
        Given Identity i
        And no active Identity Deletion Process for i exists
        When i sends a POST request to the /Identities/Self/DeletionProcesses endpoint
        Then the response status code is 201 (Created)
        And the response contains a DeletionProcess

    Scenario: There can only be one active Identity Deletion Process
        Given Identity i
        And an active deletion process for i exists
        When i sends a POST request to the /Identities/Self/DeletionProcesses endpoint
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.device.onlyOneActiveDeletionProcessAllowed"
