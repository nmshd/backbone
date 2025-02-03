@Integration
Feature: GET /Tokens

User requests multiple Tokens

    Scenario Outline: Requesting a list of Tokens in a variety of scenarios
        Given Identities i1, i2, i3 and i4
        And the following Tokens
          | tokenName | tokenOwner | forIdentity | password | allocatedBy |
          | rt1       | i1         | -           | -        | i2, i3, i4  |
          | rt2       | i2         | -           | -        | i1, i3, i4  |
          | rt3       | i1         | -           | -        | i2, i3, i4  |
          | rt4       | i2         | -           | -        | i1, i3, i4  |
          | rt5       | i1         | -           | password | i2, i3, i4  |
          | rt6       | i1         | -           | password | -           |
          | rt7       | i2         | -           | password | i1, i3, i4  |
          | rt8       | i2         | -           | password | -           |
          | rt9       | i1         | i1          | -        | -           |
          | rt10      | i2         | i3          | -        | i3          |
          | rt11      | i2         | i2          | -        | -           |
          | rt12      | i2         | i3          | -        | i3          |
          | rt13      | i2         | i3          | password | i3          |
          | rt14      | i2         | i3          | password | -           |
        When <activeIdentity> sends a GET request to the /Tokens endpoint with the following payloads
          | tokenName |
          | rt1       |
          | rt2       |
          | rt3       |
          | rt4       |
          | rt5       |
          | rt6       |
          | rt7       |
          | rt8       |
          | rt9       |
          | rt10      |
          | rt11      |
          | rt12      |
          | rt13      |
          | rt14      |
        Then the response status code is 200 (OK)
        And the response contains Token(s) <retreivedTokens>

        Examples:
          | activeIdentity | retreivedTokens                                                 |
          | i1             | rt1, rt2, rt3, rt4, rt5, rt6, rt7, rt9                          |
          | i2             | rt1, rt2, rt3, rt4, rt5, rt7, rt8, rt10, rt11, rt12, rt13, rt14 |
          | i3             | rt1, rt2, rt3, rt4, rt5, rt7, rt10, rt12, rt13                  |
          | i4             | rt1, rt2, rt3, rt4, rt5, rt7                                    |
