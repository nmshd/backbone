@Integration
Feature: PUT /Devices/Self/PushNotifications

User registers for push notifications

Scenario: Register for push notifications
	Given the user is authenticated
	When a PUT request is sent to the /Devices/Self/PushNotifications endpoint
	Then the response status code is 200 (OK)
	And the response contains the push identifier for the device
