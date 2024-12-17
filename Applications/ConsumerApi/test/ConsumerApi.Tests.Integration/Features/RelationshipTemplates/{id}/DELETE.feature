@Integration
Feature: DELETE /RelationshipTemplates/{id}

User deletes a Relationship Template

    Scenario: Deleting a template used in a relationship doesn't delete the relationship
        Given Identities i1 and i2
        And a Relationship Template t created by i1
        And an active Relationship r between i1 and i2 with template t
        When i1 sends a DELETE request to the /RelationshipTemplates/t.Id endpoint
        Then the Relationship r still exists
        And the Relationship r does not have a relationship template

    Scenario: Deleting a template actually removes it
        Given Identity i
        And a Relationship Template t created by i
        When i sends a DELETE request to the /RelationshipTemplates/t.Id endpoint
        And i sends a GET request to the /RelationshipTemplates/t.Id endpoint
        Then the response status code is 404 (Not Found)
