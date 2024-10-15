@Integration
Feature: GET /Tokens

User requests multiple Tokens

    Scenario Outline: Requesting a list of Tokens in a variety of scenarios
        Given Identities i1, i2, i3 and i4
        And the following Tokens
          | tokenName | tokenOwner | forIdentity | password |
          | rt1       | i1         | -           | -        |
          | rt2       | i2         | -           | -        |
          | rt3       | i1         | -           | -        |
          | rt4       | i2         | -           | -        |
          | rt5       | i1         | -           | password |
          | rt6       | i1         | -           | password |
          | rt7       | i2         | -           | password |
          | rt8       | i2         | -           | password |
          | rt9       | i1         | i1          | -        |
          | rt10      | i2         | i3          | -        |
          | rt11      | i2         | i2          | -        |
          | rt12      | i2         | i3          | -        |
          | rt13      | i2         | i3          | password |
          | rt14      | i2         | i3          | password |
        When <activeIdentity> sends a GET request to the /Tokens endpoint with the following payloads
          | tokenName | passwordOnGet |
          | rt1       | -             |
          | rt2       | -             |
          | rt3       | password      |
          | rt4       | password      |
          | rt5       | password      |
          | rt6       | -             |
          | rt7       | password      |
          | rt8       | -             |
          | rt9       | -             |
          | rt10      | -             |
          | rt11      | -             |
          | rt12      | -             |
          | rt13      | password      |
          | rt14      | wordpass      |
        Then the response status code is 200 (OK)
        And the response contains Token(s) <retreivedTokens>

        Examples:
          | activeIdentity | retreivedTokens                                                 |
          | i1             | rt1, rt2, rt3, rt4, rt5, rt6, rt7, rt9                          |
          | i2             | rt1, rt2, rt3, rt4, rt5, rt7, rt8, rt10, rt11, rt12, rt13, rt14 |
          | i3             | rt1, rt2, rt3, rt4, rt5, rt7, rt10, rt12, rt13                  |
          | i4             | rt1, rt2, rt3, rt4, rt5, rt7                                    |
