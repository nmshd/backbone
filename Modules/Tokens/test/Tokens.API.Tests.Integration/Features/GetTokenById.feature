﻿@Integration
Feature: GetTokenById

User requests a Token

Scenario: Requesting an own Token as an authenticated user
	Given the user is authenticated
	And an own Token t
	When a GET request is sent to the Tokens/{id} endpoint with t.Id
	Then the response status code is 200 (OK)
	And the response contains a Token

Scenario: Requesting an own Token as an anonymous user
	Given the user is unauthenticated
	And an own Token t
	When a GET request is sent to the Tokens/{id} endpoint with t.Id
	Then the response status code is 200 (OK)
	And the response contains a Token

Scenario: Requesting a peer Token as an authenticated user
	Given the user is authenticated
	And a peer Token p
	When a GET request is sent to the Tokens/{id} endpoint with p.Id
	Then the response status code is 200 (OK)
	And the response contains a Token

Scenario: Requesting a list of own Tokens
	Given the user is authenticated
	And the user created multiple Tokens
	When a GET request is sent to the Tokens endpoint with a list of ids of own Tokens
	Then the response status code is 200 (OK)
	And the response contains all Tokens with the given ids

Scenario: Requesting an own and peer tokens
	Given the user is authenticated
	And an own Token t
	And a peer Token p
	When a GET request is sent to the Tokens endpoint with a list containing t.Id, p.Id
	Then the response status code is 200 (OK)
	And the response contains both Tokens

Scenario: Requesting a nonexistent Token as an authenticated user
	Given the user is authenticated
	When a GET request is sent to the Tokens/{id} endpoint with "TOKthisisnonexisting"
	Then the response status code is 404 (Not Found)
	And the response content includes an error with the error code "error.platform.recordNotFound"

@ignore("skipping_due_to_required_backbone_changes")
Scenario: Requesting a Token with an unsupported Accept Header
	Given the Accept header is 'application/xml'
	When a GET request is sent to the Tokens/{id} endpoint with a valid Id
	Then the response status code is 406 (Not Acceptable)

@ignore("skipping_due_to_required_backbone_changes")
Scenario Outline: Requesting a Token with an invalid id
	When a GET request is sent to the Tokens/{id} endpoint with <id>
	Then the response status code is 400 (Bad Request)
	And the response content includes an error with the error code "error.platform.invalidId"
Examples:
	| id                          | description                 |
	| TOKthishastoomanycharacters | More than 20 characters     |
	| TOKnotenoughchars           | Less than 20 characters     |
	| TOK_frfssd_fdfdsed#_        | Contains invalid characters |
	| POKfdjfdjflndjkfndjk        | Does not have TOK prefix    |