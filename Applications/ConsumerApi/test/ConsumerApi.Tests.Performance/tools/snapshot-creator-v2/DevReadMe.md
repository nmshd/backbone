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
verify-json --source PerformanceTestData.xlsx --pool-config [all | <filename>]
```

##### 3.1.1.1 Methods and Tests:

- [x] Deserialize from JSON

- [x] Deserialize from Excel Calculation (PerformanceTestData.xlsx)

- [x] Verify JSON configs against Excel Calculation (PerformanceTestData.xlsx)

- [ ] Verify JSON Pool Config Command

#### 3.1.2 Generate JSON

> generates the json-pool-<worksheet-name>.json files from the source PerformanceTestData.xls

```shell
generate-json --source PerformanceTestData.xlsx --worksheet [all | <worksheet-name> ]
```

##### 3.1.2.1 Methods and Tests:

- [ ] Generate JSON configs from Excel Calculation (PerformanceTestData.xlsx)
- [ ] Generate JSON Command

#### 3.1.3 Generate RelationshipsAndMessagePoolConfigs Excel Command

> generates the RelationshipsAndMessagePoolConfigs.xlsx

```shell
generate-relationships --poolsFile pool-config.<worksheet-name>.json
```

##### 3.1.3.1 Methods and Tests:

- [ ] Generate RelationshipsAndMessagePoolConfigs Excel (RelationshipsAndMessagePoolConfigs.xlsx)
- [ ] Generate RelationshipsAndMessagePoolConfigs Excel Command

#### 3.1.4 Apply RelationshipsAndMessagePoolConfigs Excel in Database Command

> Apply Pools CSV in Database Command
> apply --baseAddress <ConsumerApi:Port> --clientId <Client-Id> --clientSecret <Client-Secret> --poolsFile pool-config.test.json --source <RelationshipsAndMessagePoolConfigs>

```shell
apply --baseAddress http://localhost:8081 --clientId test --clientSecret test --poolsFile pool-config.test.json --source RelationshipsAndMessagePoolConfigs.xlsx
```

##### 3.1.4.1 Methods and Tests:

- [ ] Apply RelationshipsAndMessagePoolConfigs Excel (Create Database Snapshot)
- [ ] Apply RelationshipsAndMessagePoolConfigs Excel in Database Command

