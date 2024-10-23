@Integration
Feature: PUT /Relationships/{id}/Reject

User rejects a Relationship

    Scenario: Reject Relationship
        Given Identities i1 and i2
        And a pending Relationship r between i1 and i2 created by i2
        When i1 sends a PUT request to the /Relationships/{r.Id}/Reject endpoint
        Then the response status code is 200 (OK)
        And the response contains a RelationshipMetadata

    Scenario: Reject Relationship to an Identity in status "ToBeDeleted"
        Given Identities i1 and i2
        And a pending Relationship r between i1 and i2 created by i2
        And i2 is in status "ToBeDeleted"
        When i1 sends a PUT request to the /Relationships/{r.Id}/Reject endpoint
        Then the response status code is 200 (OK)
        And the response contains a RelationshipMetadata
