@Integration
Feature: DELETE Device

User deletes an un-onboarded Device

Scenario: Deleting an un-onboarded Device as an authenticated user
	Given an Identity i with a device d1
	And the current user uses d1
	And an un-onboarded device d2
	When a DELETE request is sent to the Device/{id} endpoint with d2.Id
	Then the response status code is 204 (Ok)
	And d2 is deleted
