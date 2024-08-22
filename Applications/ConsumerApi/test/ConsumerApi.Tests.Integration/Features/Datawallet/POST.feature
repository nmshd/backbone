@Integration
Feature: POST Datawallet

User creates a Datawallet

	Scenario: Creating a Datawallet
		Given Identity i
		When i sends a POST request to /Datawallet endpoint with the following header:
			| Key                            | Value |
			| X-Supported-Datawallet-Version | 1     |
		Then the response status code is 201 (Created)
		And the response contains a CreateDatawalletResponse
