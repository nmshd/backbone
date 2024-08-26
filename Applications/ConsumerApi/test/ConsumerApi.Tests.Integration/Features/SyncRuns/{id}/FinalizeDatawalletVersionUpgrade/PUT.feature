@Integration
Feature: PUT SyncRuns/{id}/FinalizeDatawalletVersionUpgrade

User finalizes a DatawalletVersionUpgrade

	Scenario: Finalizing a DatawalletVersionUpgrade
		Given Identity i
		And a Datawallet dw with DatawalletVersion set to 1 created by i
		When i sends a PUT request to /SyncRuns endpoint with the SyncRunType "DatawalletVersionUpgrade"
		Then the response status code is 200 (Ok)
		And the response contains a FinalizeDatawalletVersionUpgradeResponse
