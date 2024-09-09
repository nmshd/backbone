@Integration
Feature: DELETE /Devices/Self/PushNotifications

User unregisters from push notifications

    Scenario: Unregister from push notifications
        Given Identity i
        When i sends a DELETE request to the /Devices/Self/PushNotifications endpoint
        Then the response status code is 204 (No Content)
