@Integration
Feature: POST IndividualQuota

Administrator Creates an Individual Quota

Scenario: Creating an Individual Quota for existing Identity
	Given an Identity i
	When a POST request is sent to the /Identity/{i.id}/Quotas endpoint
	Then the response status code is 201 (Created)
	And the response contains an IndividualQuota

Scenario: Creating an Individual Quota for inexistent Identity
	When a POST request is sent to the /Identity/{address}/Quotas endpoint with an inexistent identity address
	Then the response status code is 404 (Not Found)
	And the response content contains an error with the error code "error.platform.recordNotFound"

Scenario: Creating an Individual Quota for a non existent Metric Key
	Given an Identity i
	When a POST request is sent to the /Identity/{i.id}/Quotas endpoint with an invalid metric key
	Then the response status code is 400 (Bad Request)
	And the response content contains an error with the error code "error.platform.quotas.unsupportedMetricKey"
