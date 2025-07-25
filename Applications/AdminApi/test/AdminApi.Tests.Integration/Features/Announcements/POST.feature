@Integration
Feature: POST /Announcements

User create an Announcement

    Scenario: Creating an Announcement
        When a POST request is sent to the /Announcements endpoint with a valid content
        Then the response status code is 201 (Created)
        And the response contains an Announcement

    Scenario: Trying to create an Announcement without an English translation
        When a POST request is sent to the /Announcements endpoint without an English translation
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.invalidPropertyValue"

    Scenario: Creating an Announcement with an action
        When a POST request is sent to the /Announcements endpoint with an action
        Then the response status code is 201 (Created)
        And the response contains the action

    Scenario: Trying to create a non-silent Announcement with an IQL query
        When a POST request is sent to the /Announcements endpoint with isSilent=false and a non-empty IQL query
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.nonSilentAnnouncementCannotHaveIqlQuery"
