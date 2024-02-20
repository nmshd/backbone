@Integration
Feature: GET Challenge

User requests a Challenge

Scenario: Requesting a Challenge as an authenticated user
	Given the user is authenticated
	And a Challenge c
	When a GET request is sent to the Challenges/{id} endpoint with c.Id
	Then the response status code is 200 (OK)
	And the response contains a Challenge

Scenario: Requesting a Challenge as an anonymous user
	Given the user is unauthenticated
	And a Challenge c
	When a GET request is sent to the Challenges/{id} endpoint with c.Id
	Then the response status code is 401 (Unauthorized)

Scenario: Requesting a nonexistent Challenge as an authenticated user
	Given the user is authenticated
	When a GET request is sent to the Challenges/{id} endpoint with "CHLthisisnonexisting"
	Then the response status code is 404 (Not Found)

#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Requesting a Challenge with an unsupported Accept Header as an authenticated user
#	Given the user is authenticated
#	And the Accept header is 'application/xml'
#	When a GET request is sent to the Challenges/{id} endpoint with a valid Id
#	Then the response status code is 406 (Not Acceptable)
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario Outline: Requesting a Challenge with an invalid id as an authenticated user
#	Given the user is authenticated
#	When a GET request is sent to the Challenges/{id} endpoint with <id>
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.invalidId"
#Examples:
#	| id                          | description                 |
#	| CHLthishastoomanycharacters | More than 20 characters     |
#	| CHLnotenoughchars           | Less than 20 characters     |
#	| !CHLdfhuwnjdfnjnjfnd        | Contains invalid characters |
#	| CHL_frfssd_fdfdsed#_        | Contains invalid characters |
#	| PHLfdjfdjflndjkfndjk        | Does not have CHL prefix    |
