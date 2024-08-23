@Integration
Feature: PUT SyncRuns

User updates a SyncRun

	Scenario: Finalizing a DatawalletVersionUpgrade
		Given Identity i
		And a Datawallet dw with DatawalletVersion set to 1 created by i
		When i sends a PUT request to /SyncRuns endpoint with the SyncRunType "DatawalletVersionUpgrade"
		Then the response status code is 200 (Ok)
		And the response contains a FinalizeDatawalletVersionUpgradeResponse

	# The current operation only supports sync runs of type 'DatawalletVersionUpgrade'.
	# Scenario: Finalizing an ExternalEventSync

	# This test requires a finalized ExternalEventSync
	# Scenario: Refreshing expiration time
