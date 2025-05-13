@Integration
Feature: GET /.well-known/assetlinks.json

User requests assetlinks.json file

    Scenario: Requesting assetlinks.json
        When the user sends a GET request to the /.well-known/assetlinks.json endpoint
        Then the response contains the package name and the SHA256 fingerprint for the configured Android app
