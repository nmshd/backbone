@Integration
Feature: GET /docs/v2/openapi.json

User gets openapi.json v2

    Scenario: Getting Swagger UI v2
        When a GET request is sent to the /docs/v2/openapi.json endpoint
        Then the response status code is 200 (OK)
