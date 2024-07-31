# Enmeshed Performance Testing

## Database snapshots

### Generation

The generation of a snapshot is a convoluted process which can be described in the following steps:

#### 1. Creation of a RaM distribution

Using the `Identity.Pool.Creator` project, run the command with the arguemnts:

`GeneratePools --poolsFile <pools.json>`

Where `<pools.json>` is a pools configuration file. You can find examples of this file in this repository.

This will use a mix of a manual algorithm with simulated annealing to create a solution. The time it takes to run the algorith depends mostly on the number of SA iterations. That number can be changed in code.

The execution creates a `ram.csv` file which must be passed to the next step.

#### 2. Creation of entities

Using the `Identity.Pool.Creator` project, run the command with the arguemnts:

`CreateEntities --baseAddress http://localhost:8081 --clientId test --clientSecret test --poolsFile pools.json --ram selected-solution\\ram.csv`

where:

-   `--baseAddress` is the address of the ConsumerSdk.
-   `--clientId` is the client Id to use when addressing the API.
-   `--clientSecret` is the client Secret to use when addressing the API.
-   `--poolsFile` is the same file used in the step above.
-   `--ram` the Relationships & Messages configuration created in the step above (csv file).

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
cd docker-compose
.\dump_sqlserver_bak.bat
```

##### When using PostgreSQL:

```sh
cd docker-compose
.\dump_postgres.bat
```

#### 4. Persisting the results

Now that the database has been exported to a file, you can zip it and move it to a newly created folder for the snapshot you just created. You should also put the step 2. csv files there as well (entity csv files). Don't forget to update the **list** below.

### Usage

In order to use a snapshot, you must:

1.  from the snapshot:
    1. extract the entity CSV files to the place where the performance tests expect to find them.
    2. recreate the database. You may need to merge split zip files. This can be done on the Powershell by running `cmd.exe /c copy /b postgres-enmeshed.zip.* postgres-enmeshed.zip`
1.  start the consumer API
1.  run the performance tests

### List

#### Snapshot 1 (snp1.zip, downloadable from file hosting)

RaM Generation: ~8h

Entity Creation: ~10h

| Entity                | Count  |
| --------------------- | ------ |
| Identities            | 19071  |
| RelationshipTemplates | 520000 |
| Relationships         | 20376  |
| Messages              | 225553 |

#### Snapshot 2 (snp2.zip)

RaM Generation: ~20min

Entity Creation: ~1h

| Entity                | Count  |
| --------------------- | ------ |
| Identities            | 542    |
| RelationshipTemplates | 750    |
| Relationships         | 1747   |
| Messages              | 16113  |
