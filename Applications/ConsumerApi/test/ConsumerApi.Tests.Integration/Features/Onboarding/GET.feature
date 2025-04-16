@Integration
Feature: GET /Onboarding

User requests onboarding page

    Scenario: User agent is unknown html page is returned
        Given Identity i
        When a call is made to the /Onboarding endpoint
        Then the response type is text/html

    Scenario: User agent contains "Mac Os" and a redirect to the app store is returned
        Given Identity i
        When a call is made to the /Onboarding endpoint
        Then the redirection location contains appstore

    Scenario: User agent contains "Android" and a redirect to the app store is returned
        Given Identity i
        When a call is made to the /Onboarding endpoint
        Then the redirection location contains googleplay
