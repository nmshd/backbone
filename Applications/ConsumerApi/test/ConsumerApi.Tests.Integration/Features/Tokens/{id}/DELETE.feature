@Integration
Feature: DELETE /Tokens/{id}

User deletes a Token

    Scenario: Deleting a Token actually removes it
        Given Identity i
        And Token t created by i
        When i sends a DELETE request to the /Tokens/t.Id endpoint
        And i sends a GET request to the /Tokens/t.Id endpoint
        Then the response status code is 404 (Not Found)
