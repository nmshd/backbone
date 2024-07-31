@Integration
Feature: PUT Identities/Self/DeletionProcess

User cancels a deletion process

    Scenario: Canceling a deletion process
        Given Identity i
 	    And an active deletion process for i exists
 	    When i sends a PUT request to the /Identities/Self/DeletionProcesses/{id} endpoint
 	    Then the response status code is 200 (Ok)
        And the response status is 'Cancelled'
