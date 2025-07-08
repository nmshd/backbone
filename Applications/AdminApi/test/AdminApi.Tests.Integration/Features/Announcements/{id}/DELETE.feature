@Integration
Feature: DELETE /Announcements/{id}

User deletes an Announcement

    Scenario: Delete an Announcement
        Given an existing Announcement a
        When a DELETE request is sent to the /Announcements/a.Id endpoint
        Then the response status code is 204 (No Content)
        And the Announcement a does not exist anymore
