@Integration
Feature: GET /Identities/{address}/FeatureFlags

Identity tries to access feature flags of other identity with given address

    Scenario: Accessing feature flags without permisssion
        Given Identities i1 and i2
        When i2 sends a GET request to the /Identities/{address}/FeatureFlags endpoint with address=i1.address
        Then the response status code is 404 (Not Found)

    Scenario: Accessing feature flags with permission based on TemplateAllocation
        Given Identities i1 and i2
        And a Relationship Template t created by i1
        And Relationship Template t was allocated by i2
        And i1 has feature flags feature1 enabled and feature2 disabled
        When i2 sends a GET request to the /Identities/{address}/FeatureFlags endpoint with address=i1.address
        Then the response status code is 200 (OK)
        And the response contains the feature flags feature1 enabled and feature2 disabled

    Scenario: Accessing feature flags with permission based on any existing Relationship
        Given Identities i1 and i2
        And a Relationship Template t created by i1
        And an active Relationship r1 between i1 and i2 with template t
        And i2 has feature flags feature1 enabled and feature2 disabled
        When i1 sends a GET request to the /Identities/{address}/FeatureFlags endpoint with address=i2.address
        Then the response status code is 200 (OK)
        And the response contains the feature flags feature1 enabled and feature2 disabled

