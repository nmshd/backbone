@Integration
Feature: PATCH /Identities/Self/FeatureFlags

Identity changes feature flags

    Scenario: Changing feature flags
        Given Identity i
        When i sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with feature1 enabled and feature2 disabled
        Then the response status code is 204 (NoContent)
        And the Backbone has persisted feature1 as enabled and feature2 as disabled for i
