@Integration
Feature: POST /Files

Identity uploads a File

    Scenario: Uploading a File
        Given Identity i
        When i sends a POST request to the /Files endpoint
        Then the response status code is 201 (Created)
        And the response contains a CreateFileResponse
