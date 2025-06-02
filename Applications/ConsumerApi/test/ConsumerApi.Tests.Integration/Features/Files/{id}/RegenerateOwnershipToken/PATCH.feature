@Integration
Feature: Patch /Files/{id}/RegenerateOwnershipToken

Identity tries to regenerate the OwnershipToken of a file

    Scenario: The owner of a file tries to regenerate its ownership token
        Given Identities i1 and i2
        And File f created by i1
        And the ownership of f is locked by i2
        When i1 sends a PATCH request to the /Files/f.Id/RegenerateOwnershipToken endpoint
        Then the response status code is 200 (OK)
        And the response contains the new OwnershipToken of f
        And the ownership of f is not locked

    Scenario: An identity tries to regenerate the ownershipToken of an invalid FileId
        Given Identity i
        When i tries to regenerate the OwnershipToken for an invalid FileId
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.invalidPropertyValue"

    Scenario: An identity not owning the file tries to regenerate its ownership token
        Given Identities i1 and i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/RegenerateOwnershipToken endpoint
        Then the response status code is 403 (Action Forbidden)

    Scenario: An identity tries to regenerate the ownershipToken of a non existing File
        Given Identity i
        When i tries to regenerate the OwnershipToken of a non-existing FileId
        Then the response status code is 404 (Not Found)
