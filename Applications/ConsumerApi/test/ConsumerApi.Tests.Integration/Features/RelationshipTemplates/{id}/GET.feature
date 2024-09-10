@Integration
Feature: GET /RelationshipTemplates/{id}

User requests a Relationship Template

	Scenario: Requesting a Relationship Template with ForIdentity created for me
		Given Identities i1 and i2
		And Relationship Template rt created by i1 where ForIdentity is the address of i2
		When i2 sends a GET request to the /RelationshipTemplates/{id} endpoint with rt.Id
		Then the response status code is 200 (Ok)
		And the response contains a rt 

	Scenario: Requesting a Relationship Template with ForIdentity created by me
		Given Identities i1 and i2 
		And Relationship Template rt created by i1 where ForIdentity is the address of i2 
		When i1 sends a GET request to the /RelationshipTemplates/{id} endpoint with rt.Id
		Then the response status code is 200 (Ok) 
		And the response contains a rt

	Scenario: Requesting a Relationship Template with ForIdentity created for someone else
		Given Identities i1, i2 and i3
		And Relationship Template rt created by i1 where ForIdentity is the address of i2
		When i3 sends a GET request to the /RelationshipTemplates/{id} endpoint with rt.Id
		Then the response status code is 404 (Not Found)
