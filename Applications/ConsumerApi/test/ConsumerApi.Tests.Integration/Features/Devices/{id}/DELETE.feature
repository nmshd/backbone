@Integration
Feature: DELETE Device

User deletes an un-onboarded Device

	Scenario: Deleting an un-onboarded Device
		Given an Identity i with a device d1
		And an un-onboarded device d2 that belongs to i
		When i sends a DELETE request to the /Devices/{id} endpoint with d2.Id
		Then the response status code is 204 (Ok)
		And d2 of i is deleted

	Scenario: Deleting an onboarded Device is not possible
		Given an Identity i with a device d
		When i sends a DELETE request to the /Devices/{id} endpoint with d.Id
		Then the response status code is 400 (Bad Request)
		And the response content contains an error with the error code "error.platform.validation.device.deviceCannotBeDeleted"
		And d of i is not deleted

	Scenario: Deleting a non existent Device
		Given an Identity i with a device d1
		When i sends a DELETE request to the /Devices/{id} endpoint with a non existent id
		Then the response status code is 404 (Not Found)
		And the response content contains an error with the error code "error.platform.recordNotFound"
		And d1 of i is not deleted

	# baseApi is not capable to handle multiple identities
	Scenario: Deleting an un-onboarded Device of another Identity
		Given an Identity i1 with a device d1
		And an Identity i2 with devices d2 and d3
		When i1 sends a DELETE request to the /Devices/{id} endpoint with d3.Id
		Then the response status code is 400 (Bad Request)
		And d3 of i2 is not deleted
