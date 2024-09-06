@Integration
Feature: POST /RelationshipTemplates

User creates a Relationship Template

	Scenario: Creating a relationship template
		Given Identity i
		When i sends a POST request to the /RelationshipTemplates endpoint
		Then the response status code is 201 (Created)
        And the response contains a RelationshipMetadata
