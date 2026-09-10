import { RecordDuplicateError, RecordNotFoundError } from "@credo-ts/core";
import type { AgentContext, BaseRecord, BaseRecordConstructor, Query, QueryOptions, StorageService } from "@credo-ts/core";

type StoredRecord = BaseRecord;

export class InMemoryStorageService implements StorageService<StoredRecord> {
  public readonly supportsCursorPagination = false;
  private readonly records = new Map<string, StoredRecord>();

  public async save(_agentContext: AgentContext, record: StoredRecord) {
    const key = this.keyFor(record.type, record.id);

    if (this.records.has(key)) {
      throw new RecordDuplicateError(`Record with id '${record.id}' already exists.`, { recordType: record.type });
    }

    this.records.set(key, record);
  }

  public async update(_agentContext: AgentContext, record: StoredRecord) {
    const key = this.keyFor(record.type, record.id);

    if (!this.records.has(key)) {
      throw new RecordNotFoundError(`Record with id '${record.id}' was not found.`, { recordType: record.type });
    }

    this.records.set(key, record);
  }

  public async delete(_agentContext: AgentContext, record: StoredRecord) {
    await this.deleteById(_agentContext, record.constructor as BaseRecordConstructor<StoredRecord>, record.id);
  }

  public async deleteById(_agentContext: AgentContext, recordClass: BaseRecordConstructor<StoredRecord>, id: string) {
    if (!this.records.delete(this.keyFor(recordClass.type, id))) {
      throw new RecordNotFoundError(`Record with id '${id}' was not found.`, { recordType: recordClass.type });
    }
  }

  public async getById(_agentContext: AgentContext, recordClass: BaseRecordConstructor<StoredRecord>, id: string) {
    const record = this.records.get(this.keyFor(recordClass.type, id));

    if (!record) {
      throw new RecordNotFoundError(`Record with id '${id}' was not found.`, { recordType: recordClass.type });
    }

    return record;
  }

  public async getAll(_agentContext: AgentContext, recordClass: BaseRecordConstructor<StoredRecord>) {
    return Array.from(this.records.values()).filter((record) => record.type === recordClass.type);
  }

  public async findByQuery(
    agentContext: AgentContext,
    recordClass: BaseRecordConstructor<StoredRecord>,
    query: Query<StoredRecord>,
    queryOptions?: QueryOptions
  ) {
    const records = await this.getAll(agentContext, recordClass);
    const matchingRecords = records.filter((record) => this.matchesQuery(record, query));
    const offset = queryOptions?.offset ?? 0;
    const limit = queryOptions?.limit ?? matchingRecords.length;

    return matchingRecords.slice(offset, offset + limit);
  }

  private matchesQuery(record: StoredRecord, query: Query<StoredRecord>): boolean {
    const advancedQuery = query as {
      $and?: Query<StoredRecord>[];
      $not?: Query<StoredRecord>;
      $or?: Query<StoredRecord>[];
    };

    if (advancedQuery.$and) {
      return advancedQuery.$and.every((subQuery) => this.matchesQuery(record, subQuery));
    }

    if (advancedQuery.$or) {
      return advancedQuery.$or.some((subQuery) => this.matchesQuery(record, subQuery));
    }

    if (advancedQuery.$not) {
      return !this.matchesQuery(record, advancedQuery.$not);
    }

    const tags = record.getTags();
    return Object.entries(query).every(([key, value]) => tags[key] === value);
  }

  private keyFor(type: string, id: string) {
    return `${type}:${id}`;
  }
}

export class BrowserFileSystem {
  public readonly cachePath = "/";
  public readonly dataPath = "/";
  public readonly tempPath = "/";

  public async exists() {
    return false;
  }

  public async createDirectory() {
    return undefined;
  }

  public async copyFile() {
    return undefined;
  }

  public async write() {
    return undefined;
  }

  public async read() {
    return "";
  }

  public async delete() {
    return undefined;
  }

  public async downloadToFile() {
    return undefined;
  }
}
