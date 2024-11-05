@Integration
Feature: POST /Announcements

User create an Announcement

    Scenario: Creating an Announcement
        When a POST request is sent to the /Announcements endpoint with a valid content
        Then the response status code is 201 (Created)
        And the response contains an Announcement
