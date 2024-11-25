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
