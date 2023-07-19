@Integration
Feature: DELETE TierQuota

Administrator Deletes a Tier Quota

Scenario: Deleting an existing Tier Quota
	Given a Tier t
	Given a TierQuotaDefinition tq
	When a DELETE request is sent to the /Tier/{t.id}/Quotas/{tq.id} endpoint
	Then the response status code is 204 (NO CONTENT)

Scenario: Deleting a non-existent Tier Quota
	Given a Tier t
	Given an inexistent TierQuotaDefinition tq
	When a DELETE request is sent to the /Tier/{t.id}/Quotas/{tq.id} endpoint
	Then the response status code is 404 (Not Found)
	And the response content includes an error with the error code "error.platform.recordNotFound"
