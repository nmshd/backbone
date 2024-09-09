@Integration
Feature: POST /Messages

Identity sends a Message

    Scenario: Sending a Message
        Given Identities i1 and i2
        And an active Relationship r12 between i1 and i2
        When i1 sends a POST request to the /Messages endpoint with i2 as recipient
        Then the response status code is 201 (Created)
        And the response contains a SendMessageResponse

    Scenario: Sending a Message to Identity to be deleted
        Given Identities i1 and i2
        And an active Relationship r12 between i1 and i2
        And i2 is in status "ToBeDeleted"
        When i1 sends a POST request to the /Messages endpoint with i2 as recipient
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.message.recipientToBeDeleted"
        And the error contains a list of Identities to be deleted that includes i2
