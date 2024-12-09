@Integration
Feature: POST /Devices

User creates a Device

    Scenario: Registering a Device
        Given Identity i
        And a Challenge c created by i
        When i sends a POST request to the /Devices endpoint with a valid signature on c
        Then the response status code is 201 (Created)
        And the response contains a Device

    Scenario: Registering a backup Device
        Given Identity i
        And a Challenge c created by i
        When i sends a POST request to the /Devices endpoint with a valid signature on c as a backup Device
        Then the response status code is 201 (Created)
        And the response contains a Device
        And the created Device is a backup Device

    Scenario: Registering a second backup Device is not possible
        Given an Identity i with a Device d1 and a backup Device d2
        And a Challenge c created by i
        When i sends a POST request to the /Devices endpoint with a valid signature on c as a backup Device
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.device.onlyOneBackupDeviceCanExist"
