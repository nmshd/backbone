@Integration
Feature: GET /Tokens

User requests multiple Tokens

    Scenario: Requesting a list of own Tokens
        Given Identity i
        And Tokens t1 and t2 belonging to i
        When i sends a GET request to the /Tokens endpoint with the ids of t1 and t2
        Then the response status code is 200 (OK)
        And the response contains the Tokens t1 and t2

    Scenario: Requesting an own Token and a Token belonging to another identity
        Given Identities i1 and i2
        And Token t1 belonging to i1
        And Token t2 belonging to i2
        When i1 sends a GET request to the /Tokens endpoint with the ids of t1 and t2
        Then the response status code is 200 (OK)
        And the response contains the Tokens t1 and t2

    Scenario: Requesting a list of Tokens contains tokens with ForIdentity which were created by me
        Given Identities i1 and i2
        And Token t belonging to i1 where ForIdentity is the address of i2
        When i1 sends a GET request to the /Tokens endpoint with the ids of t
        Then the response status code is 200 (Ok)
        And the response contains the Token t

    Scenario: Requesting a list of Tokens contains tokens with ForIdentity which were created for me
        Given Identities i1 and i2
        And Token t belonging to i1 where ForIdentity is the address of i2
        When i2 sends a GET request to the /Tokens endpoint with the ids of t
        Then the response status code is 200 (Ok)
        And the response contains the Token t

    Scenario: Requesting a list of Tokens does not contain tokens with ForIdentity which were created for someone else
        Given Identities i1, i2 and i3
        And Token t belonging to i1 where ForIdentity is the address of i2
        When i3 sends a GET request to the /Tokens endpoint with the ids of t
        Then the response does not contain the Token t
