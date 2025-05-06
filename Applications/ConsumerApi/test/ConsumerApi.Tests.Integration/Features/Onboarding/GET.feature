@Integration
Feature: GET /resource/resourceId?appName=appName#secretKey endpoint

User requests onboarding page

    Scenario: User agent is "iPhone" and only one app-store link should be contained on the response page
        Given an http client with user agent "iPhone"
        When a call is made to the /resource/resourceId endpoint for the app "enmeshed"
        Then the response contains a link with the domain name "apps.apple.com"
        And the response does not contains a link with the domain name "play.google.com"

    Scenario: User agent is "Android" and only one app-store link should be contained on the response page
        Given an http client with user agent "Android"
        When a call is made to the /resource/resourceId endpoint for the app "enmeshed"
        Then the response contains a link with the domain name "play.google.com"
        And the response does not contains a link with the domain name "apps.apple.com"

    Scenario: User agent is unknown html page and both app-store links are contained on the response page
        Given an http client with user agent "unknown"
        When a call is made to the /resource/resourceId endpoint for the app "enmeshed"
        Then the response contains a link with the domain name "apps.apple.com"
        And the response contains a link with the domain name "play.google.com"