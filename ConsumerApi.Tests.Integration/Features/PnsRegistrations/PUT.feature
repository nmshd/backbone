@Integration
Feature: PUT /Devices/Self/PushNotifications

User creates a PnsRegistration for a device

Scenario: Creating a PnsRegistration for a device
	Given the user is authenticated
	When a PUT request is sent to the /Devices/Self/PushNotifications endpoint
