@Integration
Feature: PATCH /Tokens/{id}/ResetAccessFailedCount

Resetting the access failed count of a token

    Scenario: Trying to reset the access failed count of a non-existing token
        Given an identity with no tokens
        When a PATCH request is sent to the /Tokens/TOKANonExistingIdxxx/ResetAccessFailedCount endpoint
        Then the response status code is 404 (Not Found)
