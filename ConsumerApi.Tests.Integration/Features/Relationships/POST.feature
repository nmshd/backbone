@Integration
Feature: POST Relationship

User creates a Relationship

Scenario: Creating a Relationship
	Given Identities i1 and i2
	And a Relationship Template rt created by i1
	When a POST request is sent to the /Relationships endpoint by i2 with rt.id
	Then the response status code is 201 (Created)
	And the response contains a Relationship

Scenario: Creating a Relationship to an Identity in status "ToBeDeleted"
	Given Identities i1 and i2
	And a Relationship Template rt created by i1
	And i1 is in status "ToBeDeleted"
	When a POST request is sent to the /Relationships endpoint by i2 with rt.id
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"

Scenario: Accept Relationship Change
	Given Identities i1 and i2
	And a pending Relationship between i1 and i2 initiated by i1
	When a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Accept endpoint by i1
	Then the response status code is 200 (OK)
	And the response contains an AcceptRelationshipChangeResponse

Scenario: Accept Relationship Change to an Identity in status "ToBeDeleted"
	Given Identities i1 and i2
	And a pending Relationship between i1 and i2 initiated by i1
	And i1 is in status "ToBeDeleted"
	When a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Accept endpoint by i1
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"

Scenario: Reject Relationship Change
	Given Identities i1 and i2
	And a pending Relationship between i1 and i2 initiated by i1
	When a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Reject endpoint by i1
	Then the response status code is 200 (OK)
	And the response contains an RejectRelationshipChangeResponse

Scenario: Reject Relationship Change to an Identity in status "ToBeDeleted"
	Given Identities i1 and i2
	And a pending Relationship between i1 and i2 initiated by i1
	And i1 is in status "ToBeDeleted"
	When a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Reject endpoint by i1
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"

Scenario: Revoke Relationship Change
	Given Identities i1 and i2
	And a pending Relationship between i1 and i2 initiated by i1
	When a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Revoke endpoint by i2
	Then the response status code is 200 (OK)
	And the response contains an RevokeRelationshipChangeResponse

Scenario: Revoke Relationship Change to an Identity in status "ToBeDeleted"
	Given Identities i1 and i2
	And a pending Relationship between i1 and i2 initiated by i1
	And i1 is in status "ToBeDeleted"
	When a POST request is sent to the /Relationships/{r.Id}/Changes/{r.Changes.Id}/Revoke endpoint by i2
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"
