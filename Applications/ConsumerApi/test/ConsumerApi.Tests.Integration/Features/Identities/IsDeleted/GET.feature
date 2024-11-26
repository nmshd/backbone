@Integration
Feature: GET /Identities/IsDeleted

User wants to know whether its identity was deleted

    Scenario: Asking whether a not-yet-deleted identity was deleted
        Given an Identity i with a Device d
        And an active deletion process for i exists
        When an anonymous user sends a GET request to the /Identities/IsDeleted endpoint with d.Username
        Then the response status code is 200 (OK)
        And the response says that the identity was not deleted
        And the deletion date is not set
