@Integration
Feature: PATCH /Tokens/{id}/ResetAccessFailedCount

Listing all tokens of an identity that doesn't have any tokens
    
    Scenario: Reset a token with failed counter 0
        Given an identity with no tokens
        When a PATCH request is sent to the /Tokens/id/ResetAccessFailedCount
        Then the response status code is 200 (OK)
