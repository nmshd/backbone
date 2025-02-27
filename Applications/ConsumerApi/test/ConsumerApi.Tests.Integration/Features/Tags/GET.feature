@Integration
Feature: GET /Tags

User requests available Tags

    Scenario: Requesting the available Tags
        When A GET request to the /Tags endpoint gets sent
        Then the response status code is 200 (OK)
        And the response supports the English language
        And the response attributes contain tags

    Scenario: Requesting the tags with the current hash
        Given the most current hash h
        When A GET request to the /Tags endpoint gets sent with hash h
        Then the response status code is 304 (Not modified)
