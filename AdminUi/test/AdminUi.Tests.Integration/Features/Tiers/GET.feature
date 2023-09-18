@Integration
Feature: GET Tiers

User requests a Tier Overview List

Scenario: Requesting a list of existing Tiers
    When a GET request is sent to the /Tiers endpoint
    Then the response status code is 200 (OK)
    And the response contains a paginated list of Tiers
