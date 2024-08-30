@Integration
Feature: Change Device Password

User updates a Device

    Scenario: Changing own Device password
        Given an Identity i with a Device d
        When d sends a PUT request to the /Devices/Self/Password endpoint with the new password 'password'
        Then the response status code is 204 (No Content)
