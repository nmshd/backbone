@Integration
Feature: DELETE /RelationshipTemplates/{id}

User deletes a Relationship Template

    Scenario: Deleting a template used in a relationship doesn't delete the relationship
        Given Identities a and b
        And a Relationship Template t created by a
        And an active Relationship r between a and b with template t
        When a sends a DELETE request to the /RelationshipTemplates/t endpoint
        Then the relationship r still exists
        And the relationship r does not have a relationship template
