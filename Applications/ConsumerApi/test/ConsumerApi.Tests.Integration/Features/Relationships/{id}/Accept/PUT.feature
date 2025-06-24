@Integration
Feature: PUT /Relationships/{id}/Accept

User accepts a Relationship

    Scenario: Accept Relationship
        Given Identities i1 and i2
        And a pending Relationship r between i1 and i2 created by i2
        When i1 sends a PUT request to the /Relationships/{r.Id}/Accept endpoint
        Then the response status code is 200 (OK)
        And the response contains a RelationshipMetadata

    Scenario: Accept Relationship to an Identity in status "ToBeDeleted"
        Given Identities i1 and i2
        And a pending Relationship r between i1 and i2 created by i2
        And i2 is in status "ToBeDeleted"
        When i1 sends a PUT request to the /Relationships/{r.Id}/Accept endpoint
        Then the response status code is 200 (OK)
        And the response contains a RelationshipMetadata

    Scenario: Relationship is simoultaneously accepted and rejected
        Given a total of 100 Identities with the prefix concurrent and their id
        And pending relationships r_evenIndex between each even index with prefix concurrent and the next odd one created by the even one
        When all odd identities with prefix concurrent simultaneously send a PUT request to the /Relationships/r_evenIndex.id/Accept endpoint and a PUT request to /Relationships/r_evenIndex.Id/Reject
        Then are exactly 50 responses have code 200
