@Integration
Feature: PUT /Relationships/{id}/Reactivate/Revoke

User revokes a Relationship reactivation

    Scenario: Revoking a Relationship reactivation
        Given Identities i1 and i2
        And a terminated Relationship r between i1 and i2 with reactivation requested by i2
        When i2 sends a PUT request to the /Relationships/{r.Id}/Reactivate/Revoke endpoint
        Then the response status code is 200 (Ok)
        And the response contains a RelationshipMetadata
