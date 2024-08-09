# How to run

In order to run the performance tests, you must load an appropriate snapshot of the database. These snapshots are bundled with the usernames and passwords of the created identities/devices, meaning you can authenticate as such users and do API calls in their stead.

1.  **Install k6**

    1. You must install k6 if you haven't already. Please download it from the [official website](https://k6.io/open-source/).

1.  **Select a snapshot:**

    1. Select one of the available snapshots. You can find more information on the available snapshots in the [root README](../../README.md) file.
    1. Extract the snapshot file, and any further zip files there may be inside it.

1.  **Load the snapshot:**

    1. Place the relevant `.pg/.sql` file in the following directory: `/docker-compose/dumps/dump-files`.
    1. In the directory `/docker-compose/dumps/`, run the appropriate command: `load_postgres.bat` or `load_sqlserver_bak.bat`.
> [!CAUTION]
> This will delete you current Enmeshed Database.

1.  **Prepare the csvs:**

    1. Extract the compressed csv files into the following directory: `/Application/ConsumerApi/test/PerformanceTests/snapshots/<snapshotName>`. You must create the directory.

1.  **Start the application**

    1. Using the IDE of your choice, launch the application and ensure it can receive requests.

1.  **Run the test(s)**

    1. cd into the directory `/Application/ConsumerApi/test/PerformanceTests`
    1. Compile the typescript test files into javascript files which k6 can understand: `npx webpack`
    1. You must tweak the way the test is run to ensure it conforms with your preferences. The following CLI parameters are available:

        | Key               | Possible Values         | Notes                          |
        | ----------------- | ----------------------- | ------------------------------ |
        | `--duration`      | `60m`, `4h`, etc.       | defaults to `1h` in most cases |
        | `--address`       | `load-test.enmeshed.eu` | defaults to `localhost:8081`   |
        | `--env snapshot=` | `heavy`                 | defaults to `light`            |

    1. Run the desired command as follows:
       `k6.exe run <params> .\dist\<sxx>.test.js`

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

### Problem

**How do we know which objects even exist? Possibilities:**

1. **Using Object Identifiers in Modifications:**

    - When creating the identity (i) and its test data, save the IDs of the created messages, relationship templates, relationships, and tokens in the objectIdentifier of the modifications. After fetching the modifications, read the object IDs from the objectIdentifiers of the modifications.

2. **Saving IDs in an Additional Text File:**
    - During the creation of the identity's (i) test data, save the IDs in an additional text file. This file can then be referenced to know which objects exist.
