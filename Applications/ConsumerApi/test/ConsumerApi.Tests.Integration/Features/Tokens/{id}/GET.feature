@Integration
Feature: GET Token

User requests a Token

    Scenario: Requesting an own Token as an authenticated user
        Given Identity i
        And Token t belonging to i
        When i sends a GET request to the /Tokens/{id} endpoint with t.Id
        Then the response status code is 200 (OK)
        And the response contains a Token

    Scenario: Requesting an own Token as an anonymous user
        Given Identity i
        And Token t belonging to i
        When an anonymous user sends a GET request to the /Tokens/{id} endpoint with t.Id
        Then the response status code is 200 (OK)
        And the response contains a Token

    Scenario: Requesting a Token of another Identity as an authenticated user
        Given Identities i1 and i2
        And Token t belonging to i2
        When i1 sends a GET request to the /Tokens/{id} endpoint with t.Id
        Then the response status code is 200 (OK)
        And the response contains a Token

    Scenario: Requesting a nonexistent Token
        Given Identity i
        When i sends a GET request to the /Tokens/{id} endpoint with "TOKthisisnonexisting"
        Then the response status code is 404 (Not Found)
        And the response content contains an error with the error code "error.platform.recordNotFound"
