@Integration
Feature: PUT /Relationships/{id}/Reactivate/Revoke

User revokes a Relationship reactivation

	Scenario: Revoking a relationship reactivation
		Given Identities i1 and i2
		And a terminated Relationship r12 between i1 and i2 with reactivation request by second participant
		When i2 sends a PUT request to the /Relationships/{r12.Id}/Reactivate/Revoke endpoint
		Then the response status code is 200 (Ok)
        And the response contains a RelationshipMetadata
