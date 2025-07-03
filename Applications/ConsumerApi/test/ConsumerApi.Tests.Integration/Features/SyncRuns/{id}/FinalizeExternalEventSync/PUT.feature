@Integration
Feature: GET /SyncRuns/{id}/FinalizeExternalEventSync

    Scenario: Finalizing an external event sync before new external events were created returns the information that no new unsynced external events exist
        Given Identities i1 and i2
        And an active Relationship r between i1 and i2
        And i1 has sent a Message m1 to i2
        And 2 second(s) have passed
        And a sync run sr1 started by i2
        And 2 second(s) have passed
        When i2 finalizes sr1
        Then the response status code is 200 (OK)
        And the response contains the information that new unsynced external events do not exist

    Scenario: Finalizing an external event sync after new external events were created returns the information that new unsynced external events exist
        Given Identities i1 and i2
        And an active Relationship r between i1 and i2
        And i1 has sent a Message m1 to i2
        And 2 second(s) have passed
        And a sync run sr1 started by i2
        And i1 has sent a Message m2 to i2
        And 2 second(s) have passed
        When i2 finalizes sr1
        Then the response status code is 200 (OK)
        And the response contains the information that new unsynced external events exist

    Scenario: External events with errors are not considered "new"
        Given Identities i1 and i2
        And an active Relationship r between i1 and i2
        And i1 has sent a Message m1 to i2
        And 2 second(s) have passed
        And a sync run sr1 started by i2
        And 2 second(s) have passed
        When i2 finalizes sr1 with errors for all sync items
        Then the response status code is 200 (OK)
        And the response contains the information that new unsynced external events do not exist
