@Integration
Feature: PUT Identities/Self/DeletionProcesses/{id}/Approve

User approves a deletion process

Scenario: Approve a non-existent deletion process
    Given an Identity i
    When a PUT request is sent to the /Identities/Self/DeletionProcesses/{id}/Approve endpoint with a non-existent deletionProcessId
    Then the response status code is 404 (Not Found)
    And the response content contains an error with the error code "error.platform.recordNotFound"
