@Integration
Feature: GET Messages

Identity gets all its Messages

    Scenario: Getting all Messages
        Given Identities i1 and i2
        And a Relationship r12 between i1 and i2
        And i1 has sent a Message m1 to i2
        And i2 has sent a Message m2 to i1
        When i1 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response contains the Messages m1 and m2

    Scenario: Getting all Messages as sender after it has decomposed the Relationship
        Given Identities i1, i2 and i3
        And a Relationship r12 between i1 and i2
        And a Relationship r13 between i1 and i3
        And i1 has sent a Message m to i2 and i3
        And i1 has terminated r12
        And i1 has decomposed r12
        When i1 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response contains the Message m
        And the address of the recipient i2 is anonymized

    Scenario: Getting all Messages as sender after a recipient has decomposed the Relationship
        Given Identities i1, i2 and i3
        And a Relationship r12 between i1 and i2
        And a Relationship r13 between i1 and i3
        And i1 has sent a Message m to i2 and i3
        And i2 has terminated r12
        And i2 has decomposed r12
        When i1 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response contains the Message m

    Scenario: Getting all Messages as sender after Relationship with one recipient is fully decomposed
        Given Identities i1, i2 and i3
        And a Relationship r12 between i1 and i2
        And a Relationship r13 between i1 and i3
        And i1 has sent a Message m to i2 and i3
        And i1 has terminated r12
        And i1 has decomposed r12
        And i2 has decomposed r12
        When i1 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response contains the Message m
        And the address of the recipient i2 is anonymized

    Scenario: Getting all Messages as sender after it has decomposed Relationships with all recipients
        Given Identities i1, i2 and i3
        And a Relationship r12 between i1 and i2
        And a Relationship r13 between i1 and i3
        And i1 has sent a Message m to i2 and i3
        And i1 has terminated r12
        And i1 has decomposed r12
        And i1 has terminated r13
        And i1 has decomposed r13
        When i1 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response does not contain the Message m

    Scenario: Getting all Messages as recipient after it has decomposed its Relationship with the sender
        Given Identities i1, i2 and i3
        And a Relationship r12 between i1 and i2
        And a Relationship r13 between i1 and i3
        And i1 has sent a Message m to i2 and i3
        And i2 has terminated r12
        And i2 has decomposed r12
        When i2 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response does not contain the Message m

    Scenario: Getting all Messages as recipient after other recipient's Relationship with the sender is fully decomposed
        Given Identities i1, i2 and i3
        And a Relationship r12 between i1 and i2
        And a Relationship r13 between i1 and i3
        And i1 has sent a Message m to i2 and i3
        And i1 has terminated r12
        And i1 has decomposed r12
        And i2 has decomposed r12
        When i3 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response contains the Message m

    Scenario: Getting all Messages as sender after all Relationships are fully decomposed
        Given Identities i1, i2 and i3
        And a Relationship r12 between i1 and i2
        And a Relationship r13 between i1 and i3
        And i1 has sent a Message m to i2 and i3
        And i1 has terminated r12
        And i1 has decomposed r12
        And i2 has decomposed r12
        And i1 has terminated r13
        And i1 has decomposed r13
        And i3 has decomposed r13
        When i1 sends a GET request to the /Messages endpoint
        Then the response status code is 200 (Ok)
        And the response does not contain the Message m
