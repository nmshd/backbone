@Integration
Feature: GET /Challenges/{id}

User requests a Challenge

    Scenario: Requesting a Challenge as an authenticated user
        Given Identity i
        And a Challenge c created by i
        When i sends a GET request to the /Challenges/{id} endpoint with c.Id
        Then the response status code is 200 (OK)
        And the response contains a Challenge

    Scenario: Requesting a nonexistent Challenge
        Given Identity i
        When i sends a GET request to the /Challenges/{id} endpoint with "CHLthisisnonexisting"
        Then the response status code is 404 (Not Found)
