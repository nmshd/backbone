@Integration
Feature: DELETE Device

User deletes an un-onboarded Device

    Scenario: Deleting an un-onboarded Device
        Given an Identity i with a device d1 and an unonboarded device d2
        When d1 sends a DELETE request to the /Devices/{id} endpoint with d2.Id
        Then the response status code is 204 (No Content)
        And d2 is deleted

    Scenario: Deleting an onboarded Device is not possible
        Given an Identity i with a device d
        When d sends a DELETE request to the /Devices/{id} endpoint with d.Id
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.device.deviceCannotBeDeleted"
        And d is not deleted

    Scenario: Deleting a non existent Device
        Given an Identity i with a device d
        When d sends a DELETE request to the /Devices/{id} endpoint with a non existent id
        Then the response status code is 404 (Not Found)
        And the response content contains an error with the error code "error.platform.recordNotFound"
        And d is not deleted

    Scenario: Deleting an un-onboarded Device of another Identity is not possible
        Given an Identity i1 with a device d1
        And an Identity i2 with a device d2 and an unonboarded device d3
        When d1 sends a DELETE request to the /Devices/{id} endpoint with d2.Id
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.device.deviceCannotBeDeleted"
        And d2 is not deleted
