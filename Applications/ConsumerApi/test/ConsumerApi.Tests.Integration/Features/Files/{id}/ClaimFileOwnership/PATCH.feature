@Integration
Feature: Patch /Files/{id}/ClaimFileOwnership

Identity tries to claim the ownership of a file

    Scenario: An identity tries to claim a file using the correct ownershiptoken
        Given Identities i1 and i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with the file's ownership token
        Then the response status code is 200 (OK)
        And i2 is the new owner of f
        And the response contains the new OwnershipToken of f

    Scenario: An identity tries to claim a file using an incorrect ownershiptoken
        Given Identities i1 and i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with an incorrect ownership token
        Then the response status code is 403 (Action Forbidden)
        And the ownership of f is locked
        And i1 receives an ExternalEvent of type FileOwnershipLockedEvent which contains the id of f

    Scenario: An identity tries to claim a file with an invalid fileId
        Given Identities i1 and i2
        And File f created by i1
        When i2 tries to claim a file with an invalid FileId
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.invalidPropertyValue"

    Scenario: An identity tries to claim a file with an invalid FileOwnershipToken
        Given Identities i1 and i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with an invalid ownership token
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.invalidPropertyValue"

    Scenario: An identity tries to claim a file using the correct ownershiptoken but the file is blocked
        Given Identities i1 and i2
        And File f created by i1
        And i2 tries to claim f with a wrong token
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with the file's ownership token
        Then the response status code is 403 (Action Forbidden)

    Scenario: An identity tries to claim a file with a non-existing FileId
        Given Identity i
        When i tries to claim a non-existing file
        Then the response status code is 404 (Not Found)
