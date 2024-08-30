@Integration
Feature: PUT Identities/Self/DeletionProcesses

User cancels a deletion process

    Scenario: Canceling a deletion process
        Given Identity i
        And an active deletion process dp for i exists
        When i sends a PUT request to the /Identities/Self/DeletionProcesses/dp.Id endpoint
        Then the response status code is 200 (Ok)
        And the new status of dp is 'Cancelled'
