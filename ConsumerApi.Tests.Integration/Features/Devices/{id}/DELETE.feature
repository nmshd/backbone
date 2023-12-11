@Integration
Feature: DELETE Device

User deletes an un-onboarded Device

Scenario: Deleting an un-onboarded Device
	Given an Identity i with a device d1
	And the current user uses d1
	And an un-onboarded device d2
	When a DELETE request is sent to the Devices/{id} endpoint with d2.Id
	Then the response status code is 204 (Ok)
	And d2 is deleted

Scenario: Deleting an onboarded Device is not possible
	Given an Identity i with a device d
	And the current user uses d
	When a DELETE request is sent to the Devices/{id} endpoint with d.Id
	Then the response status code is 400 (Bad Request)
	And d is not deleted

Scenario: Deleting a non existent Device
	Given an Identity i with a device d1
	And the current user uses d1
	When a DELETE request is sent to the Devices/{id} endpoint with a non existent id
	Then the response status code is 404 (Not Found)
	And d1 is not deleted

Scenario: Deleting an un-onboarded Device of another Identity
	Given an Identity i1 with a device d1
	And the current user uses d1
	And an Identity i2 with a device d2
	When a DELETE request is sent to the Devices/{id} endpoint with d2.Id
	Then the response status code is 404 (Not Found)
	And d2 is not deleted
