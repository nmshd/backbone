@Integration
Feature: DELETE Device

User deletes an un-onboarded Device

Scenario: Deleting an un-onboarded Device as an authenticated user
	Given an Identity i with a device d1
	And the current user uses d1
	And an un-onboarded device d2
	When a DELETE request is sent to the Devices/{id} endpoint with d2.Id
	Then the response status code is 204 (Ok)
	And d2 is deleted

Scenario: Deleting an onboarded Device as an authenticated user is not possible
	Given an Identity i with a device d1
	And the current user uses d1
	When a DELETE request is sent to the Devices/{id} endpoint with d1.Id
	Then the response status code is 400 (Bad Request)
	And d1 is not deleted

Scenario: Deleting a non existant Device as an authenticated user
	Given an Identity i with a device d1
	And the current user uses d1
	When a DELETE request is sent to the Devices/{id} endpoint with a non existent id
	Then the response status code is 404 (Not Found)
	And d1 is not deleted
