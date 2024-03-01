@Integration
Feature: DELETE TierQuota

Administrator Deletes a Tier Quota

Scenario: Deleting an existing Tier Quota
	Given a Tier t with a Quota q
	When a DELETE request is sent to the /Tiers/{t.id}/Quotas/{q.id} endpoint
	Then the response status code is 204 (NO CONTENT)

Scenario: Deleting an inexistent Tier Quota
	Given a Tier t
	When a DELETE request is sent to the /Tiers/{t.id}/Quotas/{quotaId} endpoint with an inexistent quota id
	Then the response status code is 404 (Not Found)
	And the response content includes an error with the error code "error.platform.recordNotFound"
