@Integration
Feature: POST Identity

User creates an Identity

    Scenario: Creating an Identity
        Given a Challenge c created by an anonymous user
        When an anonymous user sends a POST request is sent to the /Identities endpoint with a valid signature on c
        Then the response status code is 201 (Created)
        And the response contains a CreateIdentityResponse
