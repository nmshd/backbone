# Enmeshed Performance Testing

## Database snapshots

### Generation

The generation of a snapshot is a convoluted process which can be described in the following steps:

#### 1. Creation of a _Relationships & Messages_ distribution

Using the `Identity.Pool.Creator` project, run the command with the arguemnts:

`GeneratePools --poolsFile <pools.json>`

Where `<pools.json>` is a pools configuration file. You can find examples of this file in this repository.

This will use a mix of a manual algorithm with simulated annealing to create a solution. The time it takes to run the algorith depends mostly on the number of SA iterations. That number can be changed in code.

The execution creates a `relationshipsAndMessages.csv` file which must be passed to the next step.

#### 2. Creation of entities

Using the `Identity.Pool.Creator` project, run the command with the arguemnts:

`CreateEntities --baseAddress http://localhost:8081 --clientId test --clientSecret test --poolsFile pools.json --relationshipsAndMessages selected-solution\\relationshipsAndMessages.csv`

where:

-   `--baseAddress` is the address of the ConsumerSdk.
-   `--clientId` is the client Id to use when addressing the API.
-   `--clientSecret` is the client Secret to use when addressing the API.
-   `--poolsFile` is the same file used in the step above.
-   `--relationshipsAndMessages` the Relationships & Messages configuration created in the step above (csv file).

This command will create the following entities:

-   Identities
-   Relationship Templates
-   Relationships
-   Messages
-   Challenges
-   Datawallet Modifications

It also exports a number of csv files, containing:

-   created Identities (Address, DeviceId, Username, Password, Type)
-   created Relationship Templates (IdentityAddress, RelationshipTemplateId)
-   created Relationships (RelationshipId, FromAddress, ToAddress)
-   created Messages (MessageId, AddressFrom, AddressTo)
-   created Challenges (CreatedByAddress, ChallengeId, CreatedByDevice)
-   created Datawallet Modifications (IdentityAddress, ModificationIndex, ModificationId)

The time it takes to run this command depends mostly on the number of entities to create. It is useful to reduce the logging level of the ConsumerAPI for this operation.

#### 3. Dumping the database

The specific way of doing this depends greatly on the way the database management system is running. This guide assumes docker is used.

##### When using SQL Server:

```sh
cd scripts/dumps
.\dump-sqlserver.ps1
```

##### When using PostgreSQL:

```sh
cd scripts/dumps
.\dump-postgres.ps1
```

#### 4. Persisting the results

Now that the database has been exported to a file, you can zip it and move it to a newly created folder for the snapshot you just created. You should also put the step 2. csv files there as well (entity csv files). Don't forget to update the **list** below.

### List of snapshots

#### Snapshot Heavy (snapshot.heavy.zip, downloadable from file hosting)

_Relationships & Messages_ Generation: ~8h

Entity Creation: ~10h

| Entity                | Count  |
| --------------------- | ------ |
| Identities            | 19071  |
| RelationshipTemplates | 520000 |
| Relationships         | 20376  |
| Messages              | 225553 |

#### Snapshot Light (snapshot.light.zip)

_Relationships & Messages_ Generation: ~20min

Entity Creation: ~1h

| Entity                | Count |
| --------------------- | ----- |
| Identities            | 542   |
| RelationshipTemplates | 750   |
| Relationships         | 1747  |
| Messages              | 16113 |

# How to run k6 performance tests

In order to run the performance tests, you must load an appropriate snapshot of the database. These snapshots are bundled with the usernames and passwords of the created identities/devices, meaning you can authenticate as such users and do API calls in their stead.

Test snapshots are currently only available for **Postgres**. For windows, make sure that you're using PowerShell 7. It can be installed by running `winget install --id Microsoft.PowerShell --source winget`.

1.  **Install k6**

    1. You must install k6 if you haven't already. Please download it from the [official website](https://k6.io/open-source/).

1.  **Select a snapshot:**

    1. Select one of the available snapshots. You can find more information on the available snapshots in the [scenarios README](src/scenarios/README.md) file.

1.  **Load the snapshot and the CSVs:**

    1. `cd Applications/ConsumerApi/test/ConsumerApi.Tests.Performance`
    1. Ensure that the Postgres server where the snapshot should be loaded is running.
    1. Run one of the following commands depending on what system you're using. The spanshot name **must not** contain the extension:
        1. **Linux:** `TBD`
        1. **Windows:** `# scripts/windows/load-snapshot.ps1 -SnapshotName "snapshot" [-Hostname "custom.hostname"] [-Username "dbuser"] [-Password "dbpass"] [-DbName "dbname"]`

1.  **Start the application**

    1. Using the IDE of your choice, launch the application and ensure it can receive requests.

1.  **Run the test(s)**

    1. Run one of the following commands depending on what system you're using:

        1. **Linux:** `$ scripts/linux/run-test.sh <scenario-name>`, e.g. `s01`
        1. **Windows:** `# scripts/windows/run-test.ps1 <scenario-name>`, e.g. `s01`

> [!NOTE]
> Both alternatives can be appended with `-- <extra> <parameters>`. Extra parameters are k6 parameters, some of which are explained below. 6. You must tweak the way the test is run to ensure it conforms with your preferences. The following CLI parameters are available:

    | Key                   | Default             | Possible Values                                 |
    | --------------------- | ------------------- | ----------------------------------------------- |
    | `--duration`          | depends on the test | `60m`, `4h`, etc.                               |
    | `--address`           | `localhost:8081`    | any valid URL, e.g. `load-test.enmeshed.eu`     |
    | `--env snapshot=`     | `light`             | `heavy`                                         |
    | `--env clientId=`     | `test`              | any string                                      |
    | `--env clientSecret=` | `test`              | any string                                      |

    e.g.: `$ scripts/linux/run-test.sh s01 -- --address test.enmeshed.eu:443 --duration 4h`
