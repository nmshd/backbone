@Integration
Feature: DELETE /Announcements/{id}

User deletes an Announcement

    Scenario: Delete an Announcement
        Given an existing Announcement a
        When a DELETE request is sent to the /Announcements/{id} endpoint with a.Id
        Then the response status code is 204 (No Content)
        And the Announcement a does not exist anymore

    Scenario: Trying to delete an Announcement with a non existing id
        Given an existing Announcement a
        When a DELETE request is sent to the /Announcements/{id} endpoint with a non existing id
        Then the response status code is 404 (Not Found)
