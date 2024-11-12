@Integration
Feature: DELETE /Files/{id}

User deletes a File

    Scenario: Deleting a File actually removes it
        Given Identity i
        And File f created by i
        When i sends a DELETE request to the /Files/f.Id endpoint
        And i sends a GET request to the /Files/f.Id endpoint
        Then the response status code is 404 (Not Found)
