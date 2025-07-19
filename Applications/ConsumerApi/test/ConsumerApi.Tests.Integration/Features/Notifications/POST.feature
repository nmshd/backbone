@Integration
Feature: POST /Notifications

Identity sends a Notification

    Scenario: Sending a Notification
        Given Identities i1 and i2
        And an active Relationship r12 between i1 and i2
        When i1 sends a POST request to the /Notifications endpoint with i2 as recipient
        Then the response status code is 204 (No Content)

    Scenario: Trying to send a Notification without a recipient
        Given Identities i1 and i2
        And an active Relationship r12 between i1 and i2
        When i1 sends a POST request to the /Notifications endpoint without a recipient
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.invalidPropertyValue"

    Scenario: Trying to send a Notification with a non existing code
        Given Identities i1 and i2
        And an active Relationship r12 between i1 and i2
        When i1 sends a POST request to the /Notifications endpoint with i2 as recipient and a non existing code
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.notification.codeDoesNotExist"

    Scenario: Trying to send a Notification to the recipient without a relationship
        Given Identities i1 and i2
        When i1 sends a POST request to the /Notifications endpoint with i2 as recipient
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.notification.noRelationshipToOneOrMoreRecipientsExists"

    Scenario: Trying to send a Notification to the recipient with a rejected relationship
        Given Identities i1 and i2
        And a rejected Relationship r between i1 and i2
        When i1 sends a POST request to the /Notifications endpoint with i2 as recipient
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.notification.relationshipToRecipientIsNotInCorrectStatus"
