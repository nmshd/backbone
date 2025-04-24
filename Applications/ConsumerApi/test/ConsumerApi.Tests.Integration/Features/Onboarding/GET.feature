@Integration
Feature: GET /Onboarding

User requests onboarding page

    Scenario: User agent is unknown html page is returned
        Given a http client with user agent unknown
        When a call is made to the /Onboarding endpoint
        Then the response type is text/html

    Scenario: User agent contains "Mac Os" and a redirect to the app store is returned
        Given a http client with user agent iPhone
        When a call is made to the /Onboarding endpoint
        Then the redirection location contains apps.apple.com

    Scenario: User agent contains "Android" and a redirect to the app store is returned
        Given a http client with user agent Android
        When a call is made to the /Onboarding endpoint
        Then the redirection location contains play.google.com
