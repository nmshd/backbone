#!/usr/bin/env -S npx ts-node --esm -T
import { $, ProcessOutput, echo, chalk } from "zx";
import { generate as generatePassword } from "randomstring";

$.verbose = false;

interface Module {
  username: Username;
  schemaName: SchemaName;
  additionalPermissions: SchemaPermission[];
}

// TODO:
// - refactor to have a single "setupModule", followed by some stuff that needs to be done after all modules are set up

type SchemaOperation = "USAGE";

type TableOperation = "SELECT" | "REFERENCES" | "TRIGGER" | "TRUNCATE";

type SchemaName =
  | "Challenges"
  | "Devices"
  | "Files"
  | "Messages"
  | "Quotas"
  | "Relationships"
  | "Synchronization"
  | "Tokens";

type Username =
  | "challenges"
  | "devices"
  | "files"
  | "messages"
  | "quotas"
  | "relationships"
  | "synchronization"
  | "tokens";

interface SchemaPermission {
  schemaName: SchemaName;
  schemaOperations: SchemaOperation[];
  allTablesOperations: TableOperation[];
}

const createdUsers: { username: string; password: string }[] = [];

const host = "localhost";
const port = "5432";
const adminUsername = "postgres";
const adminPassword = "admin";
const database = "enmeshed";

const modules: Module[] = [
  {
    username: "challenges",
    schemaName: "Challenges",
    additionalPermissions: [],
  },
  {
    username: "devices",
    schemaName: "Devices",
    additionalPermissions: [
      {
        schemaName: "Challenges",
        schemaOperations: ["USAGE"],
        allTablesOperations: ["SELECT"],
      },
    ],
  },
  {
    username: "files",
    schemaName: "Files",
    additionalPermissions: [],
  },
  {
    username: "messages",
    schemaName: "Messages",
    additionalPermissions: [
      {
        schemaName: "Relationships",
        schemaOperations: ["USAGE"],
        allTablesOperations: ["SELECT", "REFERENCES", "TRIGGER", "TRUNCATE"],
      },
    ],
  },
  {
    username: "quotas",
    schemaName: "Quotas",
    additionalPermissions: [],
  },
  {
    username: "relationships",
    schemaName: "Relationships",
    additionalPermissions: [],
  },
  {
    username: "synchronization",
    schemaName: "Synchronization",
    additionalPermissions: [],
  },
  {
    username: "tokens",
    schemaName: "Tokens",
    additionalPermissions: [],
  },
];

await createDatabaseIfNotExists();
await createSchemas();
await createUsers();
await setDefaultSchemasForUsers();
await grantAdditionalPermissions();
await createMigrationTables();
await setSchemaOwners();
printCreatedUsers();

async function createUsers(): Promise<void> {
  const indexOfHeading = createdUsers.length - 1;

  for (const module of modules) {
    await createUser(module.username);
  }

  if (indexOfHeading === createdUsers.length - 1) {
    createdUsers.pop();
  }
}

async function createUser(username: string): Promise<void> {
  const password = generatePassword(20);

  const sqlCommand = `CREATE USER "${username}" WITH password '${password}'`;

  const response = await runSqlCommand(sqlCommand);

  if (response.exitCode === 0) {
    createdUsers.push({ username, password });
    return;
  } else {
    if (response.stderr.includes("already exists")) {
      echo(chalk.gray(`User '${username}' already exists.`));
      return;
    }
    exitWithError(response.stderr);
  }
}

async function createSchemas(): Promise<void> {
  for (const module of modules) {
    await createSchema(module.schemaName);
  }
}

async function createSchema(schemaName: string): Promise<void> {
  const sqlCommand = `CREATE SCHEMA "${schemaName}"`;

  const response = await runSqlCommand(sqlCommand);

  if (response.exitCode !== 0) {
    if (response.stderr.includes("already exists")) {
      echo(chalk.gray(`Schema '${schemaName}' already exists.`));
      return;
    }
    exitWithError(response.stderr);
  }
}

async function setDefaultSchemasForUsers() {
  for (const module of modules) {
    await setDefaultSchemaForUser(module.username, module.schemaName);
  }
}

async function setDefaultSchemaForUser(username: string, schemaName: string) {
  const sqlCommand = `ALTER USER "${username}" SET search_path TO "${schemaName}"`;

  const response = await runSqlCommandUnsafe(sqlCommand);
}

async function runSqlCommand(sqlCommand: string, onDatabase = true) {
  try {
    return await runSqlCommandUnsafe(sqlCommand, onDatabase);
  } catch (error) {
    return error as ProcessOutput;
  }
}

async function runSqlCommandUnsafe(sqlCommand: string, onDatabase = true) {
  const databasePart = onDatabase ? `/${database}` : "";
  return await $`psql postgresql://${adminUsername}:${adminPassword}@${host}:${port}${databasePart} -c ${sqlCommand}`;
}

function printCreatedUsers() {
  if (createdUsers.length > 0) {
    echo("\n################ Created users ################");
    echo(createdUsers.map((u) => `${u.username}: ${u.password}`).join("\n"));
  }
}

async function grantAdditionalPermissions() {
  for (const module of modules) {
    await grantAdditionalPermissionsToUser(
      module.username,
      module.additionalPermissions
    );
  }
}

async function grantAdditionalPermissionsToUser(
  username: string,
  additionalPermissions: SchemaPermission[]
) {
  for (const permissions of additionalPermissions) {
    const schemaOperations = permissions.schemaOperations.join(", ");
    await runSqlCommandUnsafe(
      `GRANT ${schemaOperations} ON SCHEMA "${permissions.schemaName}" TO "${username}"`
    );

    const allTablesOperations = permissions.allTablesOperations.join(", ");
    await runSqlCommandUnsafe(
      `GRANT ${allTablesOperations} ON ALL TABLES IN SCHEMA "${permissions.schemaName}" TO "${username}"`
    );
  }
}
async function createMigrationTables() {
  for (const module of modules) {
    await createMigrationTableForModule(module);
  }
}

async function createMigrationTableForModule(module: Module) {
  const migrationsTableName = `"${module.schemaName}"."__EFMigrationsHistory"`;

  await runSqlCommandUnsafe(`CREATE TABLE IF NOT EXISTS ${migrationsTableName} (
    "MigrationId" character varying(150) COLLATE pg_catalog."default" NOT NULL,
    "ProductVersion" character varying(32) COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
  )`);

  await runSqlCommandUnsafe(
    `ALTER TABLE IF EXISTS ${migrationsTableName} OWNER to ${module.username};`
  );
}
async function setSchemaOwners() {
  for (const module of modules) {
    await setSchemaOwner(module.schemaName, module.username);
  }
}

async function setSchemaOwner(schemaName: string, username: string) {
  await runSqlCommandUnsafe(
    `ALTER SCHEMA "${schemaName}" OWNER TO "${username}"`
  );
}
async function createDatabaseIfNotExists() {
  if (!(await databaseExists())) {
    await runSqlCommandUnsafe(`CREATE DATABASE "${database}"`, false);
  }
}

async function databaseExists() {
  const sqlCommand = `SELECT datname FROM pg_database WHERE datname = '${database}'`;
  const response = await runSqlCommand(sqlCommand, false);
  if (response.exitCode === 0) {
    return response.stdout.includes(database);
  } else {
    exitWithError(response.stderr);
  }
}

function exitWithError(response: string) {
  echo(response);
  process.exit(1);
}
