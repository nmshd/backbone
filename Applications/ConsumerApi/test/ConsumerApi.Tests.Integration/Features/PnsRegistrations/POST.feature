@Integration
Feature: POST /Devices/Self/PushNotification

User sends a test push notification

	Scenario: Send a test push notification
		Given Identity i
		When i sends a POST request to the /Devices/Self/PushNotifications/SendTestNotification endpoint
		Then the response status code is 204 (No Content)
