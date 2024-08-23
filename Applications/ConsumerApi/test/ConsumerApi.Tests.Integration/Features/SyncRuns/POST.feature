@Integration
Feature: POST SyncRuns

User starts a SyncRun

	Scenario: Starting a DatawalletVersionUpgrade
		Given Identity i
		And a Datawallet dw with DatawalletVersion set to 1 created by i
		When i sends a POST request to /SyncRuns endpoint with the SyncRunType "DatawalletVersionUpgrade"
		Then the response status code is 201 (Created)
		And the response contains a StartSyncRunResponse

	Scenario: Starting an ExternalEventSync
		Given Identity i
		And a Datawallet dw with DatawalletVersion set to 1 created by i
		When i sends a POST request to /SyncRuns endpoint with the SyncRunType "ExternalEventSync"
		Then the response status code is 200 (Ok)
		And the response contains a StartSyncRunResponse
