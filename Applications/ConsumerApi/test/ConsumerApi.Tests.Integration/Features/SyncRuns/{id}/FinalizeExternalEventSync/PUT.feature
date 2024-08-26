@Integration
Feature: PUT SyncRuns/{id}/FinalizeExternalEventSync

User finalizes a ExternalEventSync

	Scenario: Finalizing an ExternalEventSync
	# Creating a relationship ensures an event necessary for the sync run to be created is available
		Given Identities i1 and i2
		And a pending Relationship r between i1 and i2 created by i2
		And a Datawallet dw with DatawalletVersion set to 1 created by i1
		When i1 sends a PUT request to /SyncRuns endpoint with the SyncRunType "ExternalEventSync"
		Then the response status code is 200 (Ok)
		And the response contains a FinalizeExternalEventSyncResponse
