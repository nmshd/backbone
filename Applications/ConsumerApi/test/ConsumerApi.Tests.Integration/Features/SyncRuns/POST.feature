@Integration
Feature: POST /SyncRuns

    Scenario: Starting a sync run is not possible if there are only blocked external events created for new messages sent while the Relationship is pending
        Given Identities i1 and i2
        And a pending Relationship r between i1 and i2
        And i1 has sent a Message m to i2
        And a sync run sr started by i2
        When i2 sends a POST request to the /SyncRuns endpoint
        Then the response status code is 400 (OK)
        And the response says that there are no new external events
