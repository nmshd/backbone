@Integration
Feature: PUT /Relationships/{id}/Reactivate/Reject

User rejects a Relationship reactivation

	Scenario: Rejecting a relationship reactivation
		Given Identities i1 and i2
		And a terminated Relationship r12 between i1 and i2 with reactivation request by second participant
		When i1 sends a PUT request to the /Relationships/{r12.Id}/Reactivate/Reject endpoint
		Then the response status code is 200 (Ok)
        And the response contains a RelationshipMetadata
