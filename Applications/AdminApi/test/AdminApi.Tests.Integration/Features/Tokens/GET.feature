@Integration
Feature: GET /Tokens?createdBy={identity-address}

Listing all tokens of an identity that doesn't have any tokens

    Scenario: Get all Tokens for an identity with no tokens
        Given an identity with no tokens
        When a GET request is sent to the /Tokens endpoint with the identity's address
        Then the response content is an empty array
