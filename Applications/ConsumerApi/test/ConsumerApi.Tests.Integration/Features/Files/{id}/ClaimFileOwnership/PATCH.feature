@Integration
Feature: Patch /Files/{id}/ClaimFileOwnership

User tries to claim the ownership of a file

    Scenario: A user tries to claim a file using the correct ownershiptoken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership/f.OwnershipToken endpoint
        Then the response status code is 200 (OK)
        And the response contains a new OwnershipToken

    Scenario: A user tries to claim a file with a non-conforming FileId
        Given Identity i
        When i sends a PATCH request to the /Files/nonExistingFile.Id/ClaimFileOwnership/nonExistingFile.OwnershipToken endpoint
        Then the response status code is 400 (Invalid Request)

    Scenario: A user tries to claim a file with a false OwnerShipToken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership/nonExistingFile.OwnershipToken endpoint
        Then the response status code is 403 (Action Forbidden)
