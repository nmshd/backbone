@Integration
Feature: POST Token

User creates a Token

    Scenario: Creating a Token as an authenticated user
        Given Identity i
        When i sends a POST request to the Tokens endpoint
        Then the response status code is 201 (Created)
        And the response contains a CreateTokenResponse

    Scenario: Creating a Token as an anonymous user
        When an anonymous user sends a POST request is sent to the Tokens endpoint
        Then the response status code is 401 (Unauthorized)
