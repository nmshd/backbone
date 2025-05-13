@Integration
Feature: GET /.well-known/apple-app-site-association

User requests apple-app-site-association file

    Scenario: Requesting apple-app-site-association
        When the user sends a GET request to the /.well-known/apple-app-site-association endpoint
        Then the response contains the app ID for the configured iOS app
