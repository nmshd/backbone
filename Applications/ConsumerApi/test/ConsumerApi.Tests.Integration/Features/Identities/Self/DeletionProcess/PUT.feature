@Integration
Feature: PUT Identities/Self/DeletionProcesses/{id}/Approve

User approves a deletion process

Scenario: Approve a non-existent deletion process
    Given Identity i
    When i sends a PUT request to the /Identities/Self/DeletionProcesses/{id}/Approve endpoint with "IDPthisisnonexisting"
    Then the response status code is 404 (Not Found)
    And the response content contains an error with the error code "error.platform.recordNotFound"
