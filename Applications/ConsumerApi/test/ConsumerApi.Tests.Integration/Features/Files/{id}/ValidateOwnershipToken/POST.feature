@Integration
Feature: POST /Files/{fileId}/ValidateOwnershipToken

Identity validates the ownership token of the specified file

    Scenario: Identity validates the ownership token of a non blocked file
        Given Identity i
        And File f created by i
        When i sends a Post request to the /Files/f.Id/ValidateOwnershipToken with the files ownership token
        Then the response status code is 200 (Created)
        And the ValidateOwnershipTokenResponse contains true

    Scenario: Identity validates the ownership token of a blocked file
        Given Identities i1 and i2
        And File f created by i1
        And i2 tries to claim f with a wrong token
        When i1 sends a Post request to the /Files/f.Id/ValidateOwnershipToken with the files ownership token
        Then the response status code is 200 (Created)
        And the ValidateOwnershipTokenResponse contains false

    Scenario: Identity tries to validate the ownership token of a file with a invalid FileId
        Given Identity i
        When i tries to validate an OwnershipToken of an invalid FileId
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.invalidPropertyValue"

    Scenario: Identity tries to validate the ownership token a file it does not own
        Given Identities i1 and i2
        And File f created by i1
        When i2 sends a Post request to the /Files/f.Id/ValidateOwnershipToken with the files ownership token
        Then the response status code is 403 (Forbidden)

    Scenario: Identity tries to validate the ownership token a file that does not exist
        Given Identity i
        When i tries to validate the token using an valid FileId of a non-existing file
        Then the response status code is 404 (Not Found)
