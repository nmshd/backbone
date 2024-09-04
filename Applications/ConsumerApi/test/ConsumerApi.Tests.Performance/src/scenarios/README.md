# Scenarios

## 01. Creating Challenges

| Property      | Configuration |
| ------------- | ------------- |
| Identity Pool | i: a+c        |
| Average Load  | 1 per 5m      |

### Steps

1. Create a Challenge

## 02. Creating Identities

| Property      | Configuration  |
| ------------- | -------------- |
| Identity Pool | not applicable |
| Average Load  | 1 per 5m       |

### Steps

1. Create a Challenge
2. Create an Identity

## 03. Creating Datawallet Modifications

| Property      | Configuration |
| ------------- | ------------- |
| Identity Pool | i: a          |
| Average Load  | 100 per 1m    |

### Steps

1. Create a Datawallet Modification (content size: 300B)

## 04. Creating Tokens

| Property      | Configuration |
| ------------- | ------------- |
| Identity Pool | i: a+c        |
| Average Load  | 1 per 10m     |

### Steps

1. Create a Token (content size: 1kB)

## 05. Creating RelationshipTemplates

| Property      | Configuration |
| ------------- | ------------- |
| Identity Pool | i: c          |
| Average Load  | 1 per 1m      |

### Steps

1. Create a RelationshipTemplate (content size: 8kB)

## 06. Relationship Lifecycle

| Property      | Configuration                      |
| ------------- | ---------------------------------- |
| Identity Pool | i1: c, i2: a (no relationship yet) |
| Average Load  | 1 every 3m                         |

### Steps

1. i1: Create a RelationshipTemplate
2. i2: Create a Relationship to i1. If a Relationship already exists, get a new i2 and try again
3. i1: Fetch Relationship
4. i1: Reject Relationship
5. i2: Fetch Relationship
6. i2: Create a Relationship to i1
7. i1: Fetch Relationship
8. i2: Revoke Relationship
9. i1: Fetch Relationship
10. i2: Create a Relationship to i1
11. i1: Fetch Relationship
12. i1: Accept Relationship
13. i2: Fetch Relationship
14. i1/i2: Terminate Relationship
15. i1/i2: Fetch Relationship
16. i1: Request Relationship reactivation
17. i2: Fetch Relationship
18. i2: Reject Relationship reactivation
19. i1: Fetch Relationship
20. i1: Request Relationship reactivation
21. i2: Fetch Relationship
22. i1: Revoke Relationship reactivation
23. i2: Fetch Relationship
24. i1: Request Relationship reactivation
25. i2: Fetch Relationship
26. i2: Accept Relationship reactivation
27. i1: Fetch Relationship
28. i1: Decompose Relationship
29. i2: Fetch Relationship
30. i2: Decompose Relationship

## 07. Sending Personalized Messages

| Property      | Configuration    |
| ------------- | ---------------- |
| Identity Pool | i1: a+c, i2: a+c |
| Average Load  | 1 every 10s      |

### Steps

1. i1: Send a Message to i2

## 08. Sending Messages to Multiple Recipients

| Property      | Configuration                                      |
| ------------- | -------------------------------------------------- |
| Identity Pool | i1: a+c, i2...in: a+c (where i1 has relationships) |
| Average Load  | 1 every 5m                                         |

### Steps

1. i1: Send a Message with n-1 recipients (i2...in, n is randomly chosen between 50 and 1000)

## 09. Connector External Event Sync

| Property      | Configuration |
| ------------- | ------------- |
| Identity Pool | i: c          |
| Average Load  | nc every 1s   |

### Steps

1. Get all datawallet modifications
2. Try to create a sync run of type ExternalEventSync
    - If the result does not contain a started sync run, abort
    - Otherwise, continue
3. Assume the sync run contains n external events
4. Finalize the sync run by:
    - Pushing n datawallet modifications
    - Sending n/5 external event results with some error code
    - Sending 4n/5 external event results without error code

## 10. Device Onboarding

| Property      | Configuration |
| ------------- | ------------- |
| Identity Pool | i: a3         |
| Average Load  | 1 every 10m   |

### Steps

1. Get all Datawallet modifications starting with index 0 (CAUTION: remember to paginate the results, if necessary!)
2. Get all Messages (CAUTION: remember to paginate the results, if necessary!)
3. Get all Relationship Templates (CAUTION: remember to paginate the results, if necessary!)
4. Get all Relationships (CAUTION: remember to paginate the results, if necessary!)
5. Get all Tokens (CAUTION: remember to paginate the results, if necessary!)
