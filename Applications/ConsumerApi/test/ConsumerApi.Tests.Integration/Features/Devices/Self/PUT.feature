@Integration
Feature: PUT /Devices/Self

User updates its Device

    Scenario: Updating own Device with valid data
        Given an Identity i with a Device d
        When d sends a PUT request to the /Devices/Self endpoint with the communication language 'de'
        Then the response status code is 204 (No Content)
        And the Backbone has persisted 'de' as the new communication language of d.

    Scenario: Updating own Device with an invalid language code as communication language
        Given an Identity i with a Device d
        When d sends a PUT request to the /Devices/Self endpoint with a non-existent language code
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.invalidDeviceCommunicationLanguage"
