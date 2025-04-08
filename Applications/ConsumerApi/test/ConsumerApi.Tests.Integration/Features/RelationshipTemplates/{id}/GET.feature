@Integration
Feature: GET /RelationshipTemplates/{id}

User requests a Relationship Template

    Scenario Outline: Requesting a Relationship Template in a variety of scenarios
        Given Identities <givenIdentities>
        And Relationship Template rt created by <templateOwner> with password "<password>" and forIdentity <forIdentity>
        When <activeIdentity> sends a GET request to the /RelationshipTemplates/rt.Id endpoint with password "<passwordOnGet>"
        Then the response status code is <responseStatusCode>

        Examples:
          | givenIdentities | templateOwner | forIdentity | password | activeIdentity | passwordOnGet | responseStatusCode | description                                                       |
          | i               | i             | -           | -        | i              | -             | 200 (OK)           | owner tries to get                                                |
          | i1 and i2       | i1            | -           | -        | i2             | -             | 200 (OK)           | non-owner tries to get                                            |
          | i               | i             | -           | -        | i              | password      | 200 (OK)           | owner passes password even though none is set                     |
          | i1 and i2       | i1            | -           | -        | i2             | password      | 200 (OK)           | non-owner identity passes password even though none is set        |
          | i               | i             | -           | password | i              | password      | 200 (OK)           | owner passes correct password                                     |
          | i               | i             | -           | password | i              | -             | 200 (OK)           | owner doesn't pass password, even though one is set               |
          | i1 and i2       | i1            | -           | password | i2             | password      | 200 (OK)           | non-owner identity passes correct password                        |
          | i1 and i2       | i1            | -           | password | i2             | -             | 404 (Not Found)    | non-owner identity passes no password even though one is set      |
          | i               | i             | i           | -        | i              | -             | 200 (OK)           | owner is forIdentity and tries to get                             |
          | i1 and i2       | i1            | i2          | -        | i1             | -             | 200 (OK)           | non-owner is forIdentity, creator tries to get                    |
          | i1 and i2       | i1            | i1          | -        | i2             | -             | 404 (Not Found)    | owner is forIdentity and non-owner tries to get                   |
          | i1 and i2       | i1            | i2          | -        | i2             | -             | 200 (OK)           | non-owner is forIdentity and tries to get                         |
          | i1 and i2       | i1            | i2          | password | i2             | password      | 200 (OK)           | non-owner is forIdentity and tries to get with correct password   |
          | i1 and i2       | i1            | i2          | password | i2             | wordpass      | 404 (Not Found)    | non-owner is forIdentity and tries to get with incorrect password |
          | i1, i2 and i3   | i1            | i2          | password | i3             | password      | 404 (Not Found)    | non-owner is forIdentity, and thirdParty tries to get             |

    Scenario: Request exhausts maximum number of allocations
        Given Identities i1 and i2
        And a Relationship Template rt created by i1 with 1 max allocations
        When i2 sends a GET request to the /RelationshipTemplates/rt.Id endpoint
        And 2 second(s) have passed
        Then the response status code is 200 (OK)
        And i1 receives an ExternalEvent of type RelationshipTemplateAllocationsExhausted which contains the id of Relationship Template rt
