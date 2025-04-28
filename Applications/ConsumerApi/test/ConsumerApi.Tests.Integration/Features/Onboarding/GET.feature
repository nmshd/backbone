@Integration
Feature: GET /Tokens/tokenId endpoint

User requests onboarding page

    Scenario: User agent is "Mac Os" and a redirect to the app store is returned
        Given a http client with user agent iPhone
        And the configuration contains a single onboarding configuration
        When a call is made to the /Tokens/tokenId endpoint
        Then the redirection location contains apps.apple.com

    Scenario: User agent is "Android" and a redirect to the app store is returned
        Given a http client with user agent Android
        And the configuration contains a single onboarding configuration
        When a call is made to the /Tokens/tokenId endpoint
        Then the redirection location contains play.google.com

    Scenario: User agent is unknown html page is returned
        Given a http client with user agent unknown
        And the configuration contains a single onboarding configuration
        When a call is made to the /Tokens/tokenId endpoint
        Then the redirection location contains Onboarding

    Scenario: User agent is unknown and app is not defined
        Given a http client with user agent unknown
        And the configuration contains a single onboarding configuration
        When a call is made to the /Tokens/tokenId endpoint
        Then the response type is text/html

    Scenario: User agent is unknown and app is defined
        Given a http client with user agent unknown
        And the configuration contains a single onboarding configuration
        When a call is made to the /Tokens/tokenId?appname=appname endpoint
        Then the response type is text/html
