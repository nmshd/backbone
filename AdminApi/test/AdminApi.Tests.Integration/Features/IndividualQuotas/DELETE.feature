@Integration
Feature: DELETE IndividualQuota

Administrator Deletes an Individual Quota

Scenario: Deleting an existing Individual Quota
	Given an Identity i with an IndividualQuota q
	When a DELETE request is sent to the /Identities/{i.address}/Quotas/{q.id} endpoint
	Then the response status code is 204 (NO CONTENT)

Scenario: Deleting an inexistent Individual Quota
	Given an Identity i
	When a DELETE request is sent to the /Identities/{i.address}/Quotas/inexistentQuotaId endpoint
	Then the response status code is 404 (Not Found)
	And the response content includes an error with the error code "error.platform.recordNotFound"
