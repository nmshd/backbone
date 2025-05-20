@Integration
Feature: Get /Files/{id}/metadata

Identity gets the file metadata

    Scenario: Getting the file metadata returns the file metadata
        Given Identity i
        And File f created by i
        When i sends a GET request to the /Files/f.Id endpoint
        Then the response status code is 200 (OK)
