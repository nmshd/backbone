@Integration
Feature: PUT /Relationships/{id}/Revoke

User revokes a Relationship

    Scenario: Revoke Relationship
        Given Identities i1 and i2
        And a pending Relationship r between i1 and i2 created by i1
        When i1 sends a PUT request to the /Relationships/{r.Id}/Revoke endpoint
        Then the response status code is 200 (OK)
        And the response contains a RelationshipMetadata

    Scenario: Revoke Relationship to an Identity in status "ToBeDeleted"
        Given Identities i1 and i2
        And a pending Relationship r between i1 and i2 created by i2
        And i2 is in status "ToBeDeleted"
        When i1 sends a PUT request to the /Relationships/{r.Id}/Revoke endpoint
        Then the response status code is 200 (OK)
        And the response contains a RelationshipMetadata
