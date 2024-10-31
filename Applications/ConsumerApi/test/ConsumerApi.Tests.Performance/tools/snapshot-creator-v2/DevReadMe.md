# ABL-221 - Fill database for performance tests

We need a basic set of identities that can be used by our tests. Those identities should already have a certain amount of messages and relationships.

The number of messages and relationships is defined in the pool-config.*.json files in the following folder: backbone/Applications/ConsumerApi/test/ConsumerApi.Tests.Performance/tools/snapshot-creator

## 1. JIRA Issue:

https://sprind-mb-team.atlassian.net/browse/ABL-221

READ Notes: https://js-soft.atlassian.net/wiki/spaces/JSSNMSHDD/pages/3892871169/Performance+Test

##2. Feature Branch

https://github.com/nmshd/backbone/tree/abl-10-fill-db-for-perftests-tool

## 3. Dev-ToDos:

### 3.1 CLI Commands:

#### 3.1.1 Verify JSON Pool Config

> verifies the json pool-config-<worksheet-name>.json files against the source PerformanceTestData.xls for validity

```shell
verify-config --source PerformanceTestData.xlsx --worksheet <worksheet-name> --pool-config pool-config.<worksheet-name>.json
```

##### 3.1.1.1 Methods and Tests:

- [x] Deserialize from JSON
- [x] Deserialize from Excel Calculation (PerformanceTestData.xlsx)
- [x] Verify JSON configs against Excel Calculation (PerformanceTestData.xlsx)
- [x] Verify JSON Pool Config Command


#### 3.1.2 Generate  Json Pool Configs with Relationships And Message Command

> generates the pool-config.<worksheet-name>.json

```shell
generate-config --source PerformanceTestData.xlsx --worksheet <worksheet-name>
```

##### 3.1.2.1 Methods and Tests:

- [x] Generate JSON configs from Excel Calculation (PerformanceTestData.xlsx)
- [x] Generate Relationships and Messages
- [x] Distribute JSON configs with Relationships and Messages (pool-config.<worksheet-name>.json)
- [x] Generate RelationshipsAndMessagePoolConfigs Excel Command

#### 3.1.3 Create Identity relationships and messages Command

> Applie the pools configs including their relationships and messages in the Database

```shell
create-snapshot --baseAddress http://localhost:8081 --clientId test --clientSecret test --pool-config pool-config.<worksheet-name>.json
```

##### 3.1.3.1 Methods and Tests:

- [ ] Apply pools configs including their relationships and messages in the Database (Create Database Snapshot)
- [ ] Apply RelationshipsAndMessagePoolConfigs Excel in Database Command

## 3.2 Console Application (CLI)

Command Line Parser Nuget we want to use: McMaster.Extensions.CommandLineUtils

nuget: https://www.nuget.org/packages/McMaster.Extensions.CommandLineUtils/

Current branch vector:

```
main --> abl-10 --> abl-10-fill-db-for-perftests-tool
```

PR (Intermediate Review) Branch: abl-10-fill-db-for-perftests-tool
PR (Intermediate Review): https://github.com/nmshd/backbone/pull/921


