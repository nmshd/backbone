@Integration
Feature: POST Datawallet/Modifications

User pushes a Datawallet Modification

	Scenario: Pushing a Datawallet Modification
		Given Identity i
		And a Datawallet dw with DatawalletVersion set to 1 created by i
		When i sends a POST request to /Datawallet/Modifications endpoint with a DatawalletModification in the payload
		Then the response status code is 201 (Created)
		And the response contains a DatawalletModification
