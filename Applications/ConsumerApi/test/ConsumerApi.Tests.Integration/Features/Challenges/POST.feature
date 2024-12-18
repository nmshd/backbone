@Integration
Feature: POST /Challenges

User creates a Challenge

    Scenario: Creating a Challenge as an anonymous user
        When an anonymous user sends a POST request to the /Challenges endpoint
        Then the response status code is 201 (Created)
        And the response contains a Challenge
        And the Challenge has an expiration date in the future
        And the Challenge does not contain information about the creator

    Scenario: Creating a Challenge as an authenticated user
        Given Identity i
        When i sends a POST request to the /Challenges endpoint
        Then the response status code is 201 (Created)
        And the response contains a Challenge
        And the Challenge has an expiration date in the future
        And the Challenge contains information about the creator
