@Integration
Feature: PUT /Relationships/{id}/Reactivate/Accept

User accepts a Relationship reactivation

Scenario: Accepting a relationship reactivation
		Given Identities i1 and i2
		And a terminated Relationship r12 between i1 and i2
		When i1 sends a PUT request to the /Relationships/{r12.Id}/Reactivate/Accept endpoint
		Then the response status code is 200 (Ok)
        And the response contains a RelationshipMetadata
