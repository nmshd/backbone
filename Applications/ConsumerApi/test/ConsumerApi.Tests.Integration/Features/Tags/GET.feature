@Integration
Feature: GET /Tags

User requests available Tags

    Scenario: Requesting the available Tags
        When A GET request to the /Tags endpoint gets sent
        Then the response status code is 200 (OK)
        And the response supports the English language
        And the response attributes contain tags
