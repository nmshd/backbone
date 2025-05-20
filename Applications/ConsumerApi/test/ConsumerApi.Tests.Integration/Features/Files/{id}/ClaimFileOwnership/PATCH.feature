@Integration
Feature: Patch /Files/{id}/ClaimFileOwnership

Identity tries to claim the ownership of a file

    Scenario: An identity tries to claim a file using the correct ownershiptoken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token f.OwnershipToken
        Then the response status code is 200 (OK)
        And the response contains the new OwnershipToken of f
        And i2 is the new owner of f

    Scenario: An identity tries to claim a file using the wrong ownershiptoken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token FILNonExistingXXXXXX.OwnershipToken
        Then the response status code is 403 (Action Forbidden)
        And it is true, that the file f has a locked ownership
        Then i1 receives an ExternalEvent e of type FileOwnershipLockedEvent which contains the address of f

    Scenario: An identity tries to claim a file with a non confomring fileId
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/NonConforming.Id/ClaimFileOwnership with token NonConforming.OwnershipToken
        Then the response status code is 400 (Bad Request)

    Scenario: An identity tries to claim a file with malformed FileOwnershipToken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with a malformed token
        Then the response status code is 400 (Bad Request)

    Scenario: An identity tries to claim a file using the correct ownershiptoken but the file is blocked
        Given Identities i1, i2
        And File f created by i1
        And i2 tries to claim f with a wrong token
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token f.OwnershipToken
        Then the response status code is 403 (Action Forbidden)

    Scenario: An identity tries to claim an exsting file with the token ownershiptoken
        Given Identities i1, i2
        And File f created by i1
        When i2 sends a PATCH request to the /Files/f.Id/ClaimFileOwnership with token FILNonExistingXXXXXX.OwnershipToken
        Then the response status code is 403 (Action Forbidden)

    Scenario: An identity tries to claim a file with a non existing FileId
        Given Identity i
        When i sends a PATCH request to the /Files/FILNonExistingXXXXXX.Id/ClaimFileOwnership with token FILNonExistingXXXXXX.OwnershipToken
        Then the response status code is 404 (Not Found)
