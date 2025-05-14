@Integration
Feature: Patch /Files/{id}/ClaimFileOwnership

User tries to claim the ownership of a file

    Scenario: A user tries to claim a file using the correct ownershiptoken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token f.OwnershipToken
        Then the response status code is 200 (OK)
        And the response contains the new OwnershipToken of f
        And i2 is the new owner of f

    Scenario: A user tries to claim a file using the wrong ownershiptoken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token FILNonExistingXXXXXX.OwnershipToken
        Then the response status code is 403 (Action Forbidden)
        And the file f is blocked for OwnershipClaims is true
        Then i1 receives an ExternalEvent e of type FileOwnershipIsLockedEvent which contains the address of f

    Scenario: A user tries to claim a file with a non confomring fileId
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/NonConforming.Id/ClaimFileOwnership with token NonConforming.OwnershipToken
        Then the response status code is 400 (Bad Request)

    Scenario: A user tries to claim a file with malformed FileOwnershipToken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with a malformed token
        Then the response status code is 400 (Bad Request)

    Scenario: A user tries to claim a file using the correct ownershiptoken but the file is blocked
        Given Identities i1, i2
        And File f created by i1
        And i2 tries to claim f with a wrong token
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token f.OwnershipToken
        Then the response status code is 403 (Action Forbidden)

    Scenario: A user tries to claim an exsting file with the token ownershiptoken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token FILNonExistingXXXXXX.OwnershipToken
        Then the response status code is 403 (Action Forbidden)

    Scenario: A user tries to claim a file with a non existing FileId
        Given Identity i
        When i sends a PATCH request to the /Files/FILNonExistingXXXXXX.Id/ClaimFileOwnership with token FILNonExistingXXXXXX.OwnershipToken
        Then the response status code is 404 (Not Found)