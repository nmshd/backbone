Feature: POST TierQuota

Administrator Creates a Tier Quota

Scenario: Creating a Tier Quota for existing Tier
	Given a Tier
	When a POST request is sent to the /Tier/{id}/Quotas endpoint
	Then the response status code is 201 (Created)
	And the response contains a TierQuotaDefinition

Scenario: Creating a Tier Quota for inexistent Tier
	Given an inexistent Tier
	When a POST request is sent to the /Tier/{id}/Quotas endpoint
	Then the response status code is 404 (Not Found)
	And the response content includes an error with the error code "error.platform.recordNotFound"