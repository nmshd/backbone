# Admin CLI

The Admin CLI is a command line interface for managing the Backbone. At the moment, the following commands exist:

## Commands

Note: all commands need a connection string and a database provider. They can be provided on two ways:

- via options of the command (see tables below)
- via the environment variables `Database__Provider` and `Database__ConnectionString`

## `backbone client list [options]`

Lists all existing OAuth clients

Options:

```
-p, --dbProvider           The database provider. Possible values: Postgres, SqlServer
-c, --dbConnectionString   The connection string to the database.
-?, -h, --help             Show help and usage information
```

## `backbone client create [options]`

Creates an OAuth client.

Options:

```
-p, --dbProvider           The database provider. Possible values: Postgres, SqlServer
-c, --dbConnectionString   The connection string to the database.
--clientId                 The clientId of the OAuth client. Default: a randomly generated string.
--clientSecret             The clientSecret of the OAuth client. Default: a randomly generated string.
--displayName              The displayName of the OAuth client. Default: the clientId.
-?, -h, --help             Show help and usage information
```

## `backbone client delete [options] <clientIds>`

Deletes the OAuth client with the given `clientId`s.

Options:

```
-p, --dbProvider           The database provider. Possible values: Postgres, SqlServer
-c, --dbConnectionString   The connection string to the database.
-?, -h, --help             Show help and usage information
```

Arguments:

A space separated list of one or more `clientId`s.
