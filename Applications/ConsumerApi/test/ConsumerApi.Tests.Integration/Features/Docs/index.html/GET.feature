@Integration
Feature: GET /docs/index.html

User gets Swagger UI

    Scenario: Getting Swagger UI
        When a GET request is sent to the /docs/index.html endpoint
        Then the response status code is 200 (OK)
