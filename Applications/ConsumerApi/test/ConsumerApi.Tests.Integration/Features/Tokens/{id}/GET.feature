@Integration
Feature: GET /Tokens/{id}

User requests a Token

    Scenario Outline: Requesting a Token in a variety of scenarios
        Given Identities <givenIdentities>
        And Token t created by <tokenOwner> with password "<password>" and forIdentity <forIdentity>
        When <activeIdentity> sends a GET request to the /Tokens/t.Id endpoint with password "<passwordOnGet>"
        Then the response status code is <responseStatusCode>

        Examples:
          | givenIdentities | tokenOwner | forIdentity | password | activeIdentity | passwordOnGet | responseStatusCode | description                                                       |
          | i               | i          | -           | -        | i              | -             | 200 (OK)           | owner tries to get                                                |
          | i1 and i2       | i1         | -           | -        | i2             | -             | 200 (OK)           | non-owner tries to get                                            |
          | i               | i          | -           | -        | -              | -             | 200 (OK)           | anonymous user tries to get                                       |
          | i               | i          | -           | -        | i              | password      | 200 (OK)           | owner passes password even though none is set                     |
          | i1 and i2       | i1         | -           | -        | i2             | password      | 200 (OK)           | non-owner identity passes password even though none is set        |
          | i               | i          | -           | -        | -              | password      | 200 (OK)           | anonymous user passes password even though none is set            |
          | i               | i          | -           | password | i              | password      | 200 (OK)           | owner passes correct password                                     |
          | i               | i          | -           | password | i              | -             | 200 (OK)           | owner doesn't pass password, even though one is set               |
          | i1 and i2       | i1         | -           | password | i2             | password      | 200 (OK)           | non-owner identity passes correct password                        |
          | i1 and i2       | i1         | -           | password | i2             | -             | 404 (Not Found)    | non-owner identity passes no password even though one is set      |
          | i               | i          | -           | password | -              | password      | 200 (OK)           | anonymous user passes correct password                            |
          | i               | i          | -           | password | -              | -             | 404 (Not Found)    | anonymous user doesn't pass password, even though one is set      |
          | i               | i          | i           | -        | i              | -             | 200 (OK)           | owner is forIdentity and tries to get                             |
          | i1 and i2       | i1         | i2          | -        | i1             | -             | 200 (OK)           | non-owner is forIdentity, creator tries to get                    |
          | i1 and i2       | i1         | i1          | -        | i2             | -             | 404 (Not Found)    | owner is forIdentity and non-owner tries to get                   |
          | i               | i          | i           | -        | -              | -             | 404 (Not Found)    | owner is forIdentity and anonymous user tries to get              |
          | i1 and i2       | i1         | i2          | -        | i2             | -             | 200 (OK)           | non-owner is forIdentity and tries to get                         |
          | i               | i          | i           | -        | -              | -             | 404 (Not Found)    | forIdentity is set and anonymous user tries to get                |
          | i1 and i2       | i1         | i2          | password | i2             | password      | 200 (OK)           | non-owner is forIdentity and tries to get with correct password   |
          | i1 and i2       | i1         | i2          | password | i2             | wordpass      | 404 (Not Found)    | non-owner is forIdentity and tries to get with incorrect password |
          | i1, i2 and i3   | i1         | i2          | password | i3             | password      | 404 (Not Found)    | non-owner is forIdentity, and thirdParty tries to get             |
          | i1 and i2       | i1         | i2          | password | -              | password      | 404 (Not Found)    | non-owner is forIdentity, and anonymous user tries to get         |
