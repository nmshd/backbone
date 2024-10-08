@Integration
Feature: GET /Relationships/CanCreate

    Scenario: Two identities without a Relationship can create one
        Given Identities i1 and i2
        When i1 sends a GET request to the /Relationships/CanCreate?peer={id} endpoint with id=i2.id
        Then the response status code is 200 (OK)
        And a Relationship can be established
        And there is no code

    Scenario: Two identities with an active Relationship can't create another one
        Given Identities i1 and i2
        And an active Relationship r between i1 and i2
        When i1 sends a GET request to the /Relationships/CanCreate?peer={id} endpoint with id=i2.id
        Then the response status code is 200 (OK)
        And a Relationship can not be established
        And the code is "error.platform.validation.relationship.relationshipToTargetAlreadyExists"

    Scenario: Two identities with a rejected Relationship can create one
        Given Identities i1 and i2
        And a rejected Relationship r between i1 and i2
        When i1 sends a GET request to the /Relationships/CanCreate?peer={id} endpoint with id=i2.id
        Then the response status code is 200 (OK)
        And a Relationship can be established
        And there is no code

    Scenario: Two identities with a rejected and an active Relationship can't create one
        Given Identities i1 and i2
        And a rejected Relationship r1 between i1 and i2
        And an active Relationship r2 between i1 and i2
        When i1 sends a GET request to the /Relationships/CanCreate?peer={id} endpoint with id=i2.id
        Then the response status code is 200 (OK)
        And a Relationship can not be established
        And the code is "error.platform.validation.relationship.relationshipToTargetAlreadyExists"

    Scenario: Two identities with two rejected Relationships can create one
        Given Identities i1 and i2
        And a rejected Relationship r1 between i1 and i2
        And a rejected Relationship r2 between i1 and i2
        When i1 sends a GET request to the /Relationships/CanCreate?peer={id} endpoint with id=i2.id
        Then the response status code is 200 (OK)
        And a Relationship can be established
        And there is no code

    Scenario: Cannot create Relationship if peer is to be deleted
        Given Identities i1 and i2
        And i2 is in status "ToBeDeleted"
        When i1 sends a GET request to the /Relationships/CanCreate?peer={id} endpoint with id=i2.id
        Then the response status code is 200 (OK)
        And a Relationship can not be established
        And the code is "error.platform.validation.relationship.peerIsToBeDeleted"
