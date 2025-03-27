@Integration
Feature: PATCH /Identities/Self/FeatureFlags

Identity changes feature flags

    Scenario: Creating feature flags
        Given Identity i
        When i sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with feature1 enabled and feature2 disabled
        Then the response status code is 204 (No Content)
        And the Backbone has persisted feature1 as enabled and feature2 as disabled for i
    
    Scenario: Updating existing feature flags
        Given Identity i
        And i has feature flags feature1 enabled and feature2 disabled
        When i sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with feature1 disabled and feature2 enabled
        Then the response status code is 204 (No Content)
        And the Backbone has persisted feature1 as disabled and feature2 as enabled for i

    Scenario: Trying to create more than the maximum number of feature flags
        Given Identity i
        When i sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with 201 features
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.featureFlag.maxNumberOfFeatureFlagsExceeded"
        And i has no feature flags

    Scenario: Trying to create an additional feature flag when maximum is already reached
        Given Identity i
        And i has 200 feature flags with name feature[1...200]
        When i sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with 1 feature flag with name feature201
        Then the response status code is 400 (Bad Request)
        And the response content contains an error with the error code "error.platform.validation.featureFlag.maxNumberOfFeatureFlagsExceeded"
        And i has 200 feature flags with names feature[1...200]

    Scenario: Peer changes feature flags
        Given Identities i1 and i2
		And a Relationship Template t created by i1
        And Relationship Template t was allocated by i2
        When i1 sends a PATCH request to the /Identities/Self/FeatureFlags endpoint with feature1 enabled and feature2 disabled
        And 2 second(s) have passed
        Then i2 receives an ExternalEvent e of type PeerFeatureFlagsChanged which contains the address of i1
    
