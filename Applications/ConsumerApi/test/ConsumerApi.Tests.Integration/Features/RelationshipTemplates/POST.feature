@Integration
Feature: POST /RelationshipTemplates

User creates a Relationship Template

	Scenario: Creating a relationship template
		Given Identity i
		When i sends a POST request to the /RelationshipTemplates endpoint
		Then the response status code is 201 (Created)
        And the response contains a RelationshipMetadata

	Scenario: Creating a relationship template with a password
		Given Identity i
		When i sends a POST request to the /RelationshipTemplates endpoint with the password "my-password"
		Then the response status code is 201 (Created)
        And the response contains a RelationshipMetadata

	Scenario: Create a personalized Relationship Template with a password
		Given Identities i1 and i2
		When i1 sends a POST request to the /RelationshipTemplate endpoint with password "my-password" and forIdentity i2
		Then the response status code is 201 (Created)
