@Integration
Feature: GET Identities/{identityAddress}/DeletionProcesses/AuditLogs

User requests the Audit Logs pertaining to all Deletion Processes of an Identity

Scenario: Requesting Audit Logs of Deletion Processes of Identity
	Given an Identity i
	And an active deletion process for Identity i exists
	When a GET request is sent to the /Identities/{i.address}/DeletionProcesses/AuditLogs endpoint
	Then the response status code is 200 (OK)
	And the response contains a list of Identity Deletion Process Audit Logs
