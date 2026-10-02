@Integration
Feature: GET /Messages/{id}

Identity gets single Message

    Scenario: Getting a Message by id
        Given Identities i1 and i2
        And an active Relationship r12 between i1 and i2
        And i1 has sent a Message m to i2
        When i2 sends a GET request to the /Message/{m.id} endpoint
        Then the response status code is 200 (Ok)

    Scenario: Getting a non existent Message
        Given Identity i
        When i sends a GET request to the /Messages/{id} endpoint with a non existent id
        Then the response status code is 404 (Not Found)
        And the response content contains an error with the error code "error.platform.recordNotFound"
