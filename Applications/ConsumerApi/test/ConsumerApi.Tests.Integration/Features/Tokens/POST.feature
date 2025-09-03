@Integration
Feature: POST /Tokens

User creates a Token

    Scenario: Creating a Token as an authenticated user
        Given Identity i
        When i sends a POST request to the /Tokens endpoint
        Then the response status code is 201 (Created)
        And the response contains a CreateTokenResponse

	Scenario: Creating a Token as an unauthenticated user
		When an anonymous user sends a POST request to the /Tokens endpoint
		Then the response status code is 201 (Created)
		And the response contains a CreateTokenResponse

    Scenario: Creating a Token with a password
		Given Identity i
		When i sends a POST request to the /Tokens endpoint with the password "password"
		Then the response status code is 201 (Created)
        And the response contains a CreateTokenResponse

	Scenario: Create a personalized Token with a password
		Given Identities i1 and i2
		When i1 sends a POST request to the /Tokens endpoint with password "password" and forIdentity i2
		Then the response status code is 201 (Created)
        And the response contains a CreateTokenResponse
