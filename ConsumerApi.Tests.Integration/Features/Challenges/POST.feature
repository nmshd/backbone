@Integration
Feature: POST Challenge

User creates a Challenge

Scenario: Creating a Challenge as an anonymous user
	Given the user is unauthenticated
	When a POST request is sent to the Challenges endpoint without authentication
	Then the response status code is 201 (Created)
	And the response contains a Challenge
	And the Challenge does not contain information about the creator

Scenario: Creating a Challenge as an authenticated user
	Given the user is authenticated
	When a POST request is sent to the Challenges endpoint
	Then the response status code is 201 (Created)
	And the response contains a Challenge
	And the Challenge contains information about the creator

#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Challenge with a JSON sent in the request content
#	When a POST request is sent to the Challenges endpoint with
#		| Key     | Value                              |
#		| Content | {"this": "is some arbitrary json"} |
#	Then the response status code is 415 (Unsupported Media Type)
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Challenge with an invalid JSON sent in the request content
#	When a POST request is sent to the Challenges endpoint with
#		| Key     | Value                             |
#		| Content | { "thisJSON": "has an extra }" }} |
#	Then the response status code is 415 (Unsupported Media Type)
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Challenge with an unsupported Content-Type header
#	When a POST request is sent to the Challenges endpoint with
#		| Key         | Value                              |
#		| ContentType | application/xml                    |
#		| Content     | <this>is some arbitrary xml</this> |
#	Then the response status code is 415 (Unsupported Media Type)
