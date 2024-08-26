@Integration
Feature: GET Relationships/CanCreate

    Scenario: Two identities without a relationship can create one
        Given Identities i1 and i2
        When a GET request is sent to the /Relationships/CanCreate?peer={i.id} endpoint by i1 for i2
        Then the response status code is 200 (OK)
        And a relationship can be established

    Scenario: Two identities with an active relationship can't create another one
        Given Identities i1 and i2
        And a Relationship r in status Active between i1 and i2 created by i1
        When a GET request is sent to the /Relationships/CanCreate?peer={i.id} endpoint by i1 for i2
        Then the response status code is 200 (OK)
        And a relationship can not be established

    Scenario: Two identities with a rejected relationship can create one
        Given Identities i1 and i2
        And a Relationship r in status Rejected between i1 and i2 created by i1
        When a GET request is sent to the /Relationships/CanCreate?peer={i.id} endpoint by i1 for i2
        Then the response status code is 200 (OK)
        And a relationship can be established

    Scenario: Two identities with a rejected and an active relationship can't create one
        Given Identities i1 and i2
        And a Relationship r1 in status Rejected between i1 and i2 created by i1
        And a Relationship r2 in status Active between i1 and i2 created by i1
        When a GET request is sent to the /Relationships/CanCreate?peer={i.id} endpoint by i1 for i2
        Then the response status code is 200 (OK)
        And a relationship can not be established

    Scenario: Two identities with two rejected relationships can create one
        Given Identities i1 and i2
        And a Relationship r1 in status Rejected between i1 and i2 created by i1
        And a Relationship r2 in status Rejected between i1 and i2 created by i1
        When a GET request is sent to the /Relationships/CanCreate?peer={i.id} endpoint by i1 for i2
        Then the response status code is 200 (OK)
        And a relationship can be established
