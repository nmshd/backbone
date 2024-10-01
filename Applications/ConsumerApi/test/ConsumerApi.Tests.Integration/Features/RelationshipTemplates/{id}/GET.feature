@Integration
Feature: GET /RelationshipTemplates/{id}

User requests a Relationship Template

    Scenario Outline: Requesting a Relationship Template in a variety of scenarios
        Given Identities <givenIdentities>
        And Relationship Template rt created by <rTempOwner> with password "<password>" and forIdentity <forIdentity>
        When <activeIdentity> sends a GET request to the /RelationshipTemplate/rt.Id endpoint with password "<passwordOnGet>"
        Then the response status code is <responseStatusCode>

        Examples:
          | givenIdentities | rTempOwner | forIdentity | password | activeIdentity | passwordOnGet | responseStatusCode | description                                                       |
          | i               | i          | -           | -        | i              | -             | 200                | owner tries to get                                                |
          | i1 and i2       | i1         | -           | -        | i2             | -             | 200                | non-owner tries to get                                            |
          | i               | i          | -           | -        | i              | password      | 200                | owner passes password even though none is set                     |
          | i1 and i2       | i1         | -           | -        | i2             | password      | 200                | non-owner identity passes password even though none is set        |
          | i               | i          | -           | password | i              | password      | 200                | owner passes correct password                                     |
          | i               | i          | -           | password | i              | -             | 200                | owner doesn't pass password, even though one is set               |
          | i1 and i2       | i1         | -           | password | i2             | password      | 200                | non-owner identity passes correct password                        |
          | i1 and i2       | i1         | -           | password | i2             | -             | 404                | non-owner identity passes no password even though one is set      |
          | i               | i          | i           | -        | i              | -             | 200                | owner is forIdentity and tries to get                             |
          | i1 and i2       | i1         | i2          | -        | i1             | -             | 200                | non-owner is forIdentity, creator tries to get                    |
          | i1 and i2       | i1         | i1          | -        | i2             | -             | 404                | owner is forIdentity and non-owner tries to get                   |
          | i1 and i2       | i1         | i2          | -        | i2             | -             | 200                | non-owner is forIdentity and tries to get                         |
          | i1 and i2       | i1         | i2          | password | i2             | password      | 200                | non-owner is forIdentity and tries to get with correct password   |
          | i1 and i2       | i1         | i2          | password | i2             | wordpass      | 404                | non-owner is forIdentity and tries to get with incorrect password |
          | i1, i2 and i3   | i1         | i2          | password | i3             | password      | 404                | non-owner is forIdentity, and thirdParty tries to get             |
