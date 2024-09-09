@Integration
Feature: PUT /Relationships/{id}/Terminate

User terminates a Relationship

	Scenario: Terminating a relationship
		Given Identities i1 and i2
		And an active Relationship r12 between i1 and i2
		When i1 sends a PUT request to the /Relationships/{r12.Id}/Terminate endpoint
		Then the response status code is 200 (Ok)
        And the response contains a RelationshipMetadata
