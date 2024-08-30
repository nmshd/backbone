@Integration
Feature: PUT /Devices/Self/Password

User changes its password

    Scenario: Changing own Device password
        Given an Identity i with a Device d
        When d sends a PUT request to the /Devices/Self/Password endpoint with the new password 'password'
        Then the response status code is 204 (No Content)
