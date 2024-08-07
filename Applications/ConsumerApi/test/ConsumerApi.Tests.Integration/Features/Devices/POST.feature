@Integration
Feature: POST Device

User creates a Device

	Scenario: Registering a Device
		Given Identity i
		And a Challenge c
		When i sends a POST request to the /Devices endpoint
		Then the response status code is 201 (Created)
		And the response contains a Device
