@Integration
Feature: POST /Devices

User creates a Device

    Scenario: Registering a Device
        Given Identity i
        And a Challenge c created by i
        When i sends a POST request to the /Devices endpoint with a valid signature on c
        Then the response status code is 201 (Created)
        And the response contains a Device
