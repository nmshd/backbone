@Integration
Feature: Patch /Files/{id}/RegenerateOwnershipToken

User tries to regenerate the OwnershipToken of a file

    Scenario: The owner of a file tries to regenerate its ownership token
        Given Identity i
        And File f created by i
        When i sends a PATCH request to the /Files/f.Id/RegenerateOwnershipToken endpoint
        Then the response status code is 200 (OK)
        And the response contains a new OwnershipToken

    Scenario: A user tries to regenerate the ownershipToken of a non-conforming FileId
        Given Identity i
        When i sends a PATCH request to the /Files/NonConforming.Id/RegenerateOwnershipToken endpoint
        Then the response status code is 400 (Invalid Request)

    Scenario: The user not owning the file tries to regenerate its ownership token
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/RegenerateOwnershipToken endpoint
        Then the response status code is 403 (Action Forbidden)

    Scenario: A user tries to regenerate the ownershipToken of a non existing File
        Given Identity i
        When i sends a PATCH request to the /Files/FILNonExistingXXXXXX.Id/RegenerateOwnershipToken endpoint
        Then the response status code is 404 (Invalid Request)