@Integration
Feature: GET /Tokens

User requests multiple Tokens

    Scenario: Requesting a list of own Tokens
        Given Identity i
        And Tokens t1 and t2 belonging to i
        When i sends a GET request to the /Tokens endpoint with the ids of t1 and t2
        Then the response status code is 200 (OK)
        And the response contains the Tokens t1 and t2

    Scenario: Requesting an own Token and a Token of a peer
        Given Identities i1 and i2
        And Token t1 belonging to i1
        And Token t2 belonging to i2
        When i1 sends a GET request to the /Tokens endpoint with the ids of t1 and t2
        Then the response status code is 200 (OK)
        And the response contains the Tokens t1 and t2
