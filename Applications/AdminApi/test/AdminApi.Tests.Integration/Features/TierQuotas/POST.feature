@Integration
Feature: POST TierQuota

Administrator Creates a Tier Quota

Scenario: Creating a Tier Quota for existing Tier
	Given a Tier t
	When a POST request is sent to the /Tiers/{t.id}/Quotas endpoint
	Then the response status code is 201 (Created)
	And the response contains a TierQuota

Scenario: Creating a Tier Quota for inexistent Tier
	When a POST request is sent to the /Tiers/{tierId}/Quotas endpoint with an inexistent tier id
	Then the response status code is 404 (Not Found)
	And the response content contains an error with the error code "error.platform.recordNotFound"

Scenario: Creating a Tier Quota for a non existent Metric Key
	Given a Tier t
	When a POST request is sent to the /Tiers/{t.id}/Quotas endpoint with an invalid metric key
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.quotas.unsupportedMetricKey"
