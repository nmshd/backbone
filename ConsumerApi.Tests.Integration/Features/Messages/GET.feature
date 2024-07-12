@Integration
Feature: GET Messages

Identity gets all its Messages

    Scenario: Getting all Messages
        Given Identities i1 and i2
        And a Relationship r12 between i1 and i2
        And i1 has sent a Message m1 to i2
        And i2 has sent a Message m2 to i1
        When i1 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response contains the Messages m1 and m2
