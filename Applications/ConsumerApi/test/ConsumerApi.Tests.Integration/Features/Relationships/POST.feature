@Integration
Feature: POST Relationships

User creates a Relationship

    Scenario: Creating a Relationship
        Given Identities i1 and i2
        And a Relationship Template rt created by i2
        When i1 sends a POST request to the /Relationships endpoint with rt.Id
        Then the response status code is 201 (Created)
        And the response contains a RelationshipMetadata

    Scenario: Creating a Relationship to an Identity in status "ToBeDeleted"
        Given Identities i1 and i2
        And a Relationship Template rt created by i2
        And i2 is in status "ToBeDeleted"
        When i1 sends a POST request to the /Relationships endpoint with rt.Id
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.relationship.peerIsToBeDeleted"
