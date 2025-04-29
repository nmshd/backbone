@Integration
Feature: GET /resource/resourceId?appName=appName#secretKey endpoint

User requests onboarding page

    Scenario: User agent is "iPhone" and only one app-store link should be contained on the response page
        Given a http client with user agent iPhone
        When a call is made to the /resource/resourceId endpoint for the enmeshed app
        Then the response contains a link containing apps.apple.com
        And the response does not contain a link containing play.google.com

    Scenario: User agent is "Android" and only one app-store link should be contained on the response page
        Given a http client with user agent Android
        When a call is made to the /resource/resourceId endpoint for the enmeshed app
        Then the response contains a link containing play.google.com
        And the response does not contain a link containing apps.apple.com

    Scenario: User agent is unknown html page and both app-store links are contained on the response page
        Given a http client with user agent unknown
        When a call is made to the /resource/resourceId endpoint for the enmeshed app
        Then the response contains a link containing apps.apple.com
        And the response contains a link containing play.google.com
