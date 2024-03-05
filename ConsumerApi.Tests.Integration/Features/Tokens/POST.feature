@Integration
Feature: POST Token

User creates a Token

Scenario: Creating a Token as an authenticated user
	Given the user is authenticated
	When a POST request is sent to the Tokens endpoint with
		| Key			| Value                                         |
		| ContentType	| application/json								|
		| Content		| {"content": "QQ==","expiresAt": "<tomorrow>"} |
	Then the response status code is 201 (Created)
	And the response contains a CreateTokenResponse

Scenario: Creating a Token as an anonymous user
	Given the user is unauthenticated
	When a POST request is sent to the Tokens endpoint with
		| Key			| Value											|
		| ContentType	| application/json								|
		| Content		| {"content": "QQ==","expiresAt": "<tomorrow>"} |
	Then the response status code is 401 (Unauthorized)

#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Token without the "Content" request content property
#	Given the user is authenticated
#	When a POST request is sent to the Tokens endpoint with
#		| Key     | Value                       |
#		| Content | {"expiresAt": "<tomorrow>"} |
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Token with an empty "Content" request content property
#	Given the user is authenticated
#	When a POST request is sent to the Tokens endpoint with
#		| Key     | Value                                     |
#		| Content | {"content": "","expiresAt": "<tomorrow>"} |
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Token with an invalid base64 in the "Content" property
#	Given the user is authenticated
#	When a POST request is sent to the Tokens endpoint with
#		| Key     | Value                                                     |
#		| Content | {"content": "<invalid base64>","expiresAt": "<tomorrow>"} |
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Token without the "expiresAt" request content property
#	Given the user is authenticated
#	When a POST request is sent to the Tokens endpoint with
#		| Key     | Value               |
#		| Content | {"content": "QQ=="} |
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Token with a date in the past in the "expiresAt" request content property
#	Given the user is authenticated
#	When a POST request is sent to the Tokens endpoint with
#		| Key     | Value                                          |
#		| Content | {"content": "QQ==","expiresAt": "<yesterday>"} |
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
#
#@ignore("skipping_due_to_required_backbone_changes")
#Scenario: Creating a Token without a request content
#	Given the user is authenticated
#	When a POST request is sent to the Tokens endpoint with no request content
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.inputCannotBeParsed"

Scenario: Creating a Token with an unsupported Content-Type
	Given the user is authenticated
	When a POST request is sent to the Tokens endpoint with
		| Key         | Value                              |
		| ContentType | application/xml                    |
		| Content     | <this>is some arbitrary xml</this> |
	Then the response status code is 415 (Unsupported Media Type)

#@ignore("skipping_due_to_required_backbone_changes")
#Scenario Outline: Creating a Token with an invalid DateTime format in the "expiresAt" request content property
#	Given the user is authenticated
#	When a POST request is sent to the Tokens endpoint with '<Content>', '<ExpiresAt>'
#	Then the response status code is 400 (Bad Request)
#	And the response content includes an error with the error code "error.platform.validation.invalidPropertyValue"
#Examples:
#	| Content | ExpiresAt  |
#	| QQ==    | 31/12/9999 |
#	| QQ==    | 13-2-9999  |
