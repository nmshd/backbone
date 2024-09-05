@Integration
Feature: PUT /Relationships/{id}/Decompose

User decomposes a terminated relationship

	Scenario: Decompose relationship as first participant
		Given Identities i1 and i2
		And a terminated Relationship r12 between i1 and i2
		When i1 sends a PUT request to the /Relationships/{r12.Id}/Decompose endpoint
		Then the response status code is 200 (Ok)
        And the response contains a RelationshipMetadata

		Scenario: Decompose relationship as second participant
		Given Identities i1 and i2
		And a terminated Relationship r12 between i1 and i2
		And i1 has decomposed r12
		When i2 sends a PUT request to the /Relationships/{r12.Id}/Decompose endpoint
		Then the response status code is 200 (Ok)
        And the response contains a RelationshipMetadata
