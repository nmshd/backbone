@Integration
Feature: PUT /Devices/Self/PushNotifications

User registers for push notifications

	Scenario: Register for push notifications
		Given Identity i
		When i sends a PUT request to the /Devices/Self/PushNotifications endpoint
		Then the response status code is 200 (OK)
		And the response contains the push identifier for the device
