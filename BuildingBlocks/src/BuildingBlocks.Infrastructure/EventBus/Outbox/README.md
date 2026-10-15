# Transactional domain-event outbox

## Architecture and scope

Each module DbContext owns `OutboxMessages` and `InboxMessages` in its existing database/schema. The shared model and interceptors are installed in `AbstractDbContextBase` **and** the separate ASP.NET Identity `DevicesDbContext`. No database connection or transaction is shared between modules. `AdminUi` also inherits the model; its current read models do not raise domain events. Its tables are migrated for model consistency but do not need a dispatcher today.

`EventHandlerService` hosts the dispatcher. It already owns the module registrations, event contracts and broker subscriptions. It now also registers Announcements so events written by AdminApi are dispatched. The same database connection settings/permissions used by a producer module must point at that module's database in EventHandlerService. Separate databases per module remain supported. All writer processes (ConsumerApi, AdminApi, jobs, event handlers, CLI) get the same persistence behavior through their DbContexts; no worker is added to HTTP applications.

The module DbContext registration selects a broker-free constructor through a delegate compiled once per context type. Legacy constructors remain callable. Merely resolving a production DbContext therefore does not initialize RabbitMQ or another broker. Direct `IEventBus` users still initialize and use their configured provider normally.

### Event lifecycle

1. An entity raises a `DomainEvent`, retaining its existing ID, creation date and payload contract.
2. Before SaveChanges, the interceptor adds an outbox row to **that same DbContext**. JSON serialization uses the same System.Text.Json defaults as the providers. The event name uses the existing naming extension; the CLR type uses its full name and simple assembly name, without assembly-version coupling.
3. EF saves business rows and outbox rows atomically in its normal local transaction. Staging performs no reads and does not call SaveChanges recursively. Inserting additional rows can change EF batching and whether an explicit database transaction is needed; no constant roundtrip count is promised.
4. After a successful implicit commit, the saved events are removed from their entities. With an explicit transaction they remain until its commit. Failed saves retain the staged rows/event identities for retry. Rollback restores the tracked states/original values needed for another save, including generated values. Disposal without commit is detected at the next save. Transaction delegates must still be replay-safe: arbitrary application-side mutations, network calls or newly generated IDs inside a retry delegate cannot be generically undone.
5. The dispatcher atomically claims due rows, publishes outside any database transaction through the existing `IEventBus`, and conditionally marks them published using its lease token.
6. On failure the row is retained, its next attempt is scheduled, and its lease is released. A restart resumes from the same tables. An expired lease is reclaimable.

All four synchronous/asynchronous SaveChanges overloads flow through the same interceptors. With `SaveChanges(false)`, callers must still accept/reset **business** tracker state before saving again, as required by EF. The interceptor marks successfully saved outbox rows unchanged to avoid reinserting them. Ambient System.Transactions and disabling EF transactions are rejected when entity events are staged; use EF-managed local transactions. `RunInTransaction` joins an existing transaction when called by an inbox handler. In that case the outer owner controls isolation/retries; a failed inner operation must propagate and roll back that transaction. SaveChanges' affected-entry count includes outbox entries.

Bulk/ExecuteUpdate/ExecuteDelete operations do not run SaveChanges interceptors or discover entity events. Direct broker publication outside this persistence path remains direct; for example, the existing explicit publication in Devices' DeleteTier handler is outside the outbox's atomicity guarantee. The four Relationships consumer fan-out handlers use the application abstraction `IEventOutbox<RelationshipsModule>` to enqueue outgoing events in their local context instead of publishing before their inbox transaction commits. The generic module marker selects the matching context without coupling handlers to a persistence adapter.

## Claims, retries and ordering

PostgreSQL uses a CTE with `FOR UPDATE SKIP LOCKED` and `UPDATE ... RETURNING`. SQL Server uses an updatable CTE with `UPDLOCK, READPAST, READCOMMITTEDLOCK` and `OUTPUT INSERTED`. Claim/update statements are short autocommit operations. `READCOMMITTEDLOCK` supports the read-committed-snapshot configuration as well. Leases use database time and random ownership tokens; old owners cannot acknowledge a row reclaimed by another worker.

A heartbeat renews a batch's leases every third of the configured lease interval while the batch is publishing. Ownership is checked again before each publish, including work queued behind the configured parallelism limit. Renewal failure stops starting additional work; already running broker calls cannot be fenced or canceled through the existing IEventBus interface. Slow/broken connections therefore depend on the providers' network timeouts. During a process pause or partition longer than the lease, overlapping network calls and duplicate delivery remain possible. An indefinitely stuck broker call can also prolong shutdown until the host/orchestrator timeout; terminating the process leaves its rows recoverable after lease expiry.

`Attempts` counts **claims**, including attempts interrupted before publishing. Retry delay uses capped exponential backoff with jitter between 50% and 100% of the cap for that attempt. After the configured attempt limit a row is marked `FailedAt` and excluded from ordinary polling. It remains stored for investigation/replay; the publisher never silently deletes it. Invalid contracts/JSON follow the same isolation policy. Initial EventHandlerService startup still uses the existing broker initialization and connection retries; configure service restart on startup failure. Retry/lease recovery is durable; an outage longer than the retry budget requires operator replay.

Streams are hashes of the entity type and primary-key values. Events enqueued directly by fan-out handlers supply an explicit stream. Only a stream's earliest unpublished event is eligible, including while leased or backing off. A dead event blocks its successors until repaired or explicitly discarded. This avoids retry-induced overtaking while allowing independent entities/modules to publish in parallel. `SaveId` and `Position` preserve event order within a SaveChanges batch even if EF sorts INSERT commands by primary key; the lowest database `Sequence` for each stream/save group orders distinct saves. This also avoids cyclic predecessor dependencies when concurrent INSERT batches interleave. Current event-producing entities allocate domain IDs before saving. New entities with store-generated keys need an explicitly stable stream for ordering across saves; temporary EF keys do not provide that guarantee.

There is **no global order**, no common order across different entities of an aggregate or different modules, and no serializable aggregate-version order across concurrently committing transactions. Brokers/consumer queues also do not guarantee processing order across event types. Existing dependent handlers (for example, quota identity creation requiring a tier) still depend on their broker retries. Workflows requiring a stronger cross-entity/concurrent-transaction order need explicit aggregate versions and consumer gap handling; this implementation does not invent such a contract.

## Delivery and consumer idempotency

The guarantee is **at least once**, subject to retry/dead-letter intervention and correct durable broker configuration. A crash after broker acceptance but before the outbox acknowledgement causes republication of the original event ID/payload. RabbitMQ publisher channels now enable tracked publisher confirmations; Azure Service Bus and Google Pub/Sub retain their awaited SDK send/publish calls. Confirmation means broker acceptance, not that every consumer has processed the event. Exchange/topic/subscription durability, routing and retention remain deployment responsibilities; RabbitMQ's existing non-mandatory routing behavior is retained.

All three provider consumption paths invoke `InboxDispatcher`. For registered module handlers it reserves the unique `(EventId, Handler)` key and executes the handler in the same local DbContext transaction. Handler saves, outbox fan-out and the receipt commit together. Concurrent duplicates contend on the receipt's unique key; a losing delivery is retried by the broker and then observes the committed receipt. Execution-strategy retries create a fresh scope/handler/DbContext. The inbox strategy includes unique-index conflicts (up to 15 retries, capped at one second), preserving the synchronization index allocator's retry behavior when its transaction is joined; it also resolves concurrent receipt reservations. Exhausted conflicts propagate to normal broker retry/dead-letter handling. Unknown/custom non-module handlers keep the previous dispatch behavior and must supply their own idempotency.

The inbox protects local database side effects, including synchronization external-event creation. It **cannot** make push notifications or other external I/O atomic: a crash after that side effect but before the inbox commit can repeat it. Those services need their own idempotency if duplicates are unacceptable. Inbox transactions currently span the handler, including such calls, so long push operations can hold the inbox transaction open; the **outbox publisher** holds no database transaction across broker calls. Direct IEventBus usage elsewhere is not automatically intercepted. No exactly-once claim is made.

## Configuration

EventHandlerService accepts an `Outbox` configuration section (also via `Outbox__...` environment variables):

```json
{
  "Outbox": {
    "BatchSize": 50,
    "Parallelism": 4,
    "PollIntervalMilliseconds": 1000,
    "LeaseSeconds": 120,
    "MaxAttempts": 20,
    "InitialRetrySeconds": 2,
    "MaxRetrySeconds": 3600
  }
}
```

These are the defaults. Batch size is limited to 1–1000, parallelism to 1–64, polling to at least 10 ms and leases to at least 15 seconds. Invalid configuration fails startup. Each module has one polling loop per service instance; parallelism is **per module**. Bound memory is proportional to module count × batch size × event size. Choose batch size, network timeouts, lease length, connection-pool sizes and replica count together. A non-empty batch is followed immediately by another claim; empty/error polls wait. Database failures are retried by the next poll; broker failures are scheduled per row. Existing broker-internal retries still run inside an outbox attempt.

## Telemetry and operations

The meter/activity source `enmeshed.backbone.outbox` is registered with the existing OpenTelemetry setup. Instruments use only the bounded `module` tag:

| Instrument suffix | Meaning |
| --- | --- |
| `pending` | All unpublished rows, including isolated failures |
| `oldest.age` | Age in seconds of the oldest unpublished row |
| `published` | Successfully persisted publishing acknowledgements |
| `failed` | Failed publishing attempts |
| `retries` | Processing attempts after the first claim |
| `publishing.duration` | Broker/acknowledgement processing duration in seconds |
| `delivery.delay` | Time from staging until persisted publish acknowledgement |

Backlog gauges refresh about every 30 seconds per replica. They represent the same database backlog, so aggregate across replicas using **max**, not sum. Counters are per-replica and can be summed. Failed rows/blocked streams increase backlog age; alert on age, failures and sustained backlog growth. TraceParent/TraceState and correlation ID are stored and restored for the provider's normal propagation. Arbitrary baggage is not persisted to avoid copying sensitive attributes. Logs contain the random outbox row ID, error type, module and attempt; LastError stores only the error type. Neither contains payload/exception text or connection strings.

The live-work indexes are filtered on unpublished/nonfailed rows and on `(Stream, Sequence)` for predecessor checks. `InboxMessages.ProcessedAt` is indexed for maintenance. Plan regular bounded retention work outside the dispatcher:

- Archive/delete **published** outbox rows after an appropriate operational/audit retention period, and only when no unpublished row of the same `SaveId` remains. Keeping a live save group intact preserves its ordering anchor.
- Keep pending/failed rows until they are replayed or deliberately discarded.
- Retain inbox receipts at least as long as every possible broker/outbox/manual replay horizon. Removing a receipt permits that event to be processed again. Indefinite replay requires indefinite receipts or business-level idempotency.

Replay procedure: investigate the error type and deploy a compatible/fixed contract first; with publishers stopped or after confirming no unexpired lease, reset `FailedAt`, `LastError`, `Attempts`, `LeaseId` and `LeaseExpiresAt`, and set `NextAttemptAt` to current UTC/database time. Leave ID, EventId, Payload, Stream, Sequence, SaveId and Position unchanged. Discarding an event requires an explicit audited operator decision (marking it processed/skipped releases its stream) and forfeits delivery of that event. There is intentionally no unauthenticated replay API and no automatic data deletion.

## Migrations and rollout

There are twenty additive `AddTransactionalOutbox` migrations: PostgreSQL and SQL Server for Announcements, Challenges, Devices, Files, Messages, Quotas, Relationships, Synchronization, Tokens and AdminUi. Each adds the two tables and four indexes; existing business data is not rewritten. Apply using DatabaseMigrator, with enough DDL permissions. Run publishers with normal data permissions afterwards. Design-time DbContext factories allow scaffolding without starting the executable migrator or contacting a database.

A safe cutover is:

1. Back up and apply all additive schema migrations before deploying any new writer/consumer. Keep broker queues/subscriptions intact.
2. For a transition with no **additional** legacy commit/publish gap, pause incoming writes/jobs and drain old producers and in-flight handlers. Events already lost by the old implementation cannot be reconstructed by this migration.
3. Upgrade **all EventHandlerService replicas** first, including the inbox, Announcements database access and dispatcher. Verify subscriptions, database connectivity, metrics and permissions. Avoid mixed old/new consumer replicas for the same queues: old handlers do not write inbox receipts.
4. Upgrade all writers and resume traffic. Each new SaveChanges writes only to its outbox; it does not also publish directly. There is no dual-write toggle. Previously queued events are processed with the new inbox.
5. During a rolling producer upgrade, old writers still use their legacy synchronous path and retain its original failure window. New writers use only the outbox. Both wire contracts remain compatible, but a zero-loss claim for legacy writes during that mixed interval would be false. Use the pause/drain procedure when that window is unacceptable.

Rollback: keep the tables and a compatible new dispatcher running until pending/leased/failed rows are accounted for. Stop/drain new writers before switching writers back. Do not roll back the additive migrations or remove the dispatcher while outbox work remains. Keep a dispatcher version able to deserialize pending contracts; renaming event types/assemblies or changing JSON contracts requires compatibility handling. A version unable to deserialize an event will retry/isolate it, not silently drop it.

## Verification and performance

`OutboxPersistenceTests` cover sync/async overloads, atomic failure, explicit rollback/commit, disposal without commit, retry, SaveChanges(false), and broker-free context registration. `OutboxPublisherTests` run against isolated PostgreSQL/SQL Server databases and cover durable retries/restarts, disjoint concurrent claims/publishers, expired leases/stale acknowledgements, crash-after-ack duplicates, stream ordering (including reversed INSERT key order), poison isolation, slow-publish renewal, trace/correlation restoration and transactional inbox handling. `OutboxMigrationTests` applies all migrations in the migrator's order and checks runtime model consistency and both tables for every context. RabbitMQ's optional compatibility test verifies the persisted wire contract through the real confirmed publisher/consumer path. Existing broker naming and domain serialization tests remain relevant; Azure/GCP end-to-end tests require their respective test services/credentials.

Run with `BACKBONE_OUTBOX_TEST_POSTGRES`, `BACKBONE_OUTBOX_TEST_SQLSERVER` and optionally `BACKBONE_OUTBOX_TEST_RABBITMQ` pointing at **dedicated test services**. Database tests need permission to create/drop their own uniquely named test databases; they never delete an existing application database. Missing connections cause explicit test skips.

```sh
dotnet run --project BuildingBlocks/test/BuildingBlocks.Infrastructure.Tests -- -namespace 'Backbone.BuildingBlocks.Infrastructure.Tests.EventBus*'
dotnet run --project Applications/DatabaseMigrator/test/DatabaseMigrator.Tests -- -class '*OutboxMigrationTests'
```

The existing ConsumerApi k6 identity-creation scenario now emits p50, p95 and p99. For a before/after comparison, use identical snapshot data, SDK/runtime, database/broker resources, load and telemetry settings on an isolated deployment; restore/reset test data between runs. Test both healthy and delayed/unavailable brokers. Record HTTP percentile distributions and throughput, then outbox backlog/delivery delay separately. Count database command batches and begin/commit operations per request using EF command/transaction instrumentation (EF command spans alone omit transaction roundtrips). Separate request traffic from dispatcher/inbox traffic and exclude startup/warmup. Additional INSERTs may fit an existing batch or require extra commands; requests without entity events should not perform outbox reads.

No ConsumerApi before/after latency or roundtrip benchmark is claimed by this change. Local component tests establish semantics, not HTTP performance. Local verification used PostgreSQL 16 and RabbitMQ 4, including complete PostgreSQL migrations and confirmed RabbitMQ delivery. Both providers' runtime models and idempotent migration scripts were checked. The SQL Server container failed during startup under x64 emulation on the ARM host, so SQL Server claim/publish execution remains unverified. Azure Service Bus and Google Pub/Sub end-to-end tests were not run. Verify those paths on suitable infrastructure before production rollout.
