@Integration
Feature: POST Relationship

User creates a Relationship

	Scenario: Creating a Relationship
		Given Identities i1 and i2
		And a Relationship Template rt created by i2
		When i1 sends a POST request to the /Relationships endpoint with rt.id
		Then the response status code is 201 (Created)
		And the response contains a RelationshipResponse

	Scenario: Creating a Relationship to an Identity in status "ToBeDeleted"
		Given Identities i1 and i2
		And a Relationship Template rt created by i2
		And i2 is in status "ToBeDeleted"
		When i1 sends a POST request to the /Relationships endpoint with rt.id
		Then the response status code is 400 (Bad Request)
		And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"

	Scenario: Accept Relationship
		Given Identities i1 and i2
		And a Relationship r in status Pending between i1 and i2 created by i2
		When i1 sends a POST request to the /Relationships/{r.Id}/Accept endpoint
		Then the response status code is 200 (OK)
		And the response contains a RelationshipResponse

	Scenario: Accept Relationship to an Identity in status "ToBeDeleted"
		Given Identities i1 and i2
		And a Relationship r in status Pending between i1 and i2 created by i2
		And i2 is in status "ToBeDeleted"
		When i1 sends a POST request to the /Relationships/{r.Id}/Accept endpoint
		Then the response status code is 400 (Bad Request)
		And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"

	Scenario: Reject Relationship
		Given Identities i1 and i2
		And a Relationship r in status Pending between i1 and i2 created by i2
		When i1 sends a POST request to the /Relationships/{r.Id}/Reject endpoint
		Then the response status code is 200 (OK)
		And the response contains a RelationshipResponse

	Scenario: Reject Relationship to an Identity in status "ToBeDeleted"
		Given Identities i1 and i2
		And a Relationship r in status Pending between i1 and i2 created by i2
		And i2 is in status "ToBeDeleted"
		When i1 sends a POST request to the /Relationships/{r.Id}/Reject endpoint
		Then the response status code is 400 (Bad Request)
		And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"

	Scenario: Revoke Relationship
		Given Identities i1 and i2
		And a Relationship r in status Pending between i1 and i2 created by i1
		When i1 sends a POST request to the /Relationships/{r.Id}/Revoke endpoint
		Then the response status code is 200 (OK)
		And the response contains a RelationshipResponse

	Scenario: Revoke Relationship to an Identity in status "ToBeDeleted"
		Given Identities i1 and i2
		And a Relationship r in status Pending between i1 and i2 created by i1
		And i2 is in status "ToBeDeleted"
		When i1 sends a POST request to the /Relationships/{r.Id}/Revoke endpoint
		Then the response status code is 400 (Bad Request)
		And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"
