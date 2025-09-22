@Integration
Feature: GET /Tags

User requests available Tags

    Scenario: Requesting the available Tags
        When A GET request to the /Tags endpoint gets sent
        Then the response status code is 200 (OK)
        And the response supports the English language
        And the response attributes contain tags

    Scenario: Requesting the tags with the current hash
        Given a list of tags l with an ETag e
        And l didn't change since the last fetch
        When A GET request to the /Tags endpoint gets sent with the If-None-Match header set to e
        Then the response status code is 304 (Not modified)
        And the response content is empty
