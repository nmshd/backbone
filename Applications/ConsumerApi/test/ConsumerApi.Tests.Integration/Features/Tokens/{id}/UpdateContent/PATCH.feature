@Integration
Feature: PATCH /Tokens/{id}/UpdateContent

Update the content of a Token

    Scenario: Updating the content of a Token
        Given Identity i
        And Token t created by an anonymous user
        When i sends a POST request to the /Tokens/t.Id/UpdateContent endpoint
        Then the response status code is 200 (Ok)
        And the Token t has the new content
