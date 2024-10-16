@Integration
Feature: GET /SyncRuns/{id}/ExternalEvents

#[x] 2 neue Spalten für External Events:
#    [x] Context; it can be used for other external event types as well;
#           and when querying, we could have a method called MessageReceivedExternalEvent.WasSentInRelationship(relationshipId)
#           returning an expression that checks for "Context == relationshipsId"
#    [x] IsDeliveryBlocked
#[x] wenn IsDeliveryBlocked == true, dann Event nicht zu Sync Run hinzufügen
#[ ] bei MessageCreatedDomainEvent
#    [x] 1. IsDeliveryBlocked des neuen Events auf "Relationships[event.RelationshipId].Status == Terminated" setzen
#    [x] 2. Context auf event.RelationshipId setzen
#[ ] bei RelationshipStatusChangedDomainEvent
#    [x] wenn NewStatus == Active, dann alle Events mit RelationshipId auf IsDeliveryBlocked = false setzen
#    [x] wenn NewStatus == DeletionProposed, dann alle Events mit RelationshipId von Initiator löschen
#    [x] wenn NewStatus == ReadyForDeletion, dann alle Events mit RelationshipId von Initiator löschen

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
        And a sync run sr started by i2
        When i2 sends a GET request to the /SyncRuns/sr.id/ExternalEvents endpoint
        Then the response status code is 200 (OK)
        Then the response contains an external event for the Message m

    Scenario: Getting external events returns events for messages sent before the Relationship was terminated
        Given Identities i1 and i2
        And an active Relationship r between i1 and i2
        And i1 has sent a Message m to i2
        And i1 has terminated r
        And a sync run sr started by i2
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
