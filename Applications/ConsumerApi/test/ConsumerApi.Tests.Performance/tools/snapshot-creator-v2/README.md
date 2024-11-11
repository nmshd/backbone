# ABL-221 - Fill database for performance tests

We need a basic set of identities that can be used by our tests. Those identities should already have a certain amount of messages and relationships.

The number of messages and relationships is defined in the pool-config.*.json files in the following folder: backbone/Applications/ConsumerApi/test/ConsumerApi.Tests.Performance/tools/snapshot-creator

## 1. JIRA Issue:

https://sprind-mb-team.atlassian.net/browse/ABL-244

READ Notes: https://js-soft.atlassian.net/wiki/spaces/JSSNMSHDD/pages/3892871169/Performance+Test

## 2. Feature Branch

https://github.com/nmshd/backbone/tree/abl-10-fill-db-for-perftests-tool

Current branch vector:

```
main --> abl-10 --> abl-10-fill-db-for-perftests-tool
```

PR (Intermediate Review) Branch: abl-10-fill-db-for-perftests-tool
PR (Intermediate Review): https://github.com/nmshd/backbone/pull/921

## 3. CLI Commands Usage:

### 3.1 Verify JSON Pool Config

> verifies the json pool-config-<worksheet-name>.json files against the source PerformanceTestData.xls for validity

```shell
verify-config --source PerformanceTestData.xlsx --worksheet <worksheet-name> --pool-config pool-config.<worksheet-name>.json
```


### 3.2 Generate Json Pool Config

> generates the pool-config.<worksheet-name>.json

```shell
generate-config --source PerformanceTestData.xlsx --worksheet <worksheet-name>
```

### 3.3 Create Database Snapshot

> Applie the pools configs including their relationships and messages in the Database

```shell
create-snapshot --baseAddress http://localhost:8081 --clientId test --clientSecret test --pool-config pool-config.<worksheet-name>.json
```

