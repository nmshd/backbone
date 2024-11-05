@Integration
Feature: GET /Announcements

User gets all Announcements

    Scenario: Get all Announcement
        When a GET request is sent to the /Announcements endpoint
        Then the response status code is 200 (OK)
        And the response contains a list of Announcements
