@Integration
Feature: GET /SyncRuns/{id}/ExternalEvents

    Scenario: Getting external events does not return events for messages sent while the Relationship is still terminated
        Given Identities i1 and i2
        And a terminated Relationship r between i1 and i2
        And i1 has sent a Message m to i2
        And a sync run sr started by i2
        When i2 sends a GET request to the /SyncRuns/sr.id/ExternalEvents endpoint
        Then the response status code is 200 (OK)
        And the response does not contain an external event for the Message m

    Scenario: Getting external events returns events for messages sent while the Relationship was terminated
        Given Identities i1 and i2
        And a terminated Relationship r between i1 and i2
        And i1 has sent a Message m to i2
        And r was fully reactivated
        And 1 second(s) have passed
        And a sync run sr started by i2
        When i2 sends a GET request to the /SyncRuns/sr.id/ExternalEvents endpoint
        Then the response status code is 200 (OK)
        Then the response contains an external event for the Message m

    Scenario: Getting external events returns events for messages sent before the Relationship was terminated
        Given Identities i1 and i2
        And an active Relationship r between i1 and i2
        And i1 has sent a Message m to i2
        And i1 has terminated r
        And 10 second(s) have passed
        And a sync run sr started by i2
        And 10 second(s) have passed
        When i2 sends a GET request to the /SyncRuns/sr.id/ExternalEvents endpoint
        Then the response status code is 200 (OK)
        Then the response contains an external event for the Message m

    Scenario: Getting external events does not return events for messages that were sent with an old Relationship
        Given Identities i1 and i2
        And an active Relationship r between i1 and i2
        And i1 has sent a Message m to i2
        And i1 has terminated r
        And i1 has decomposed r
        And i2 has decomposed r
        And an active Relationship r between i1 and i2
        And a sync run sr started by i2
        When i2 sends a GET request to the /SyncRuns/sr.id/ExternalEvents endpoint
        Then the response status code is 200 (OK)
        Then the response does not contain an external event for the Message m
