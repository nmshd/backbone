# Admin CLI

The Admin CLI is a command line interface for managing the Backbone.

## How to use

Currently we only deploy the Admin CLI as Docker image. In order to run it, you can, for example, use the following command:

```
docker run -it ghcr.io/nmshd/backbone-admin-cli:<tag>
```

This will open a shell in the container. From there, you can run the tool with the `backbone` command. You can see all available commands by running `backbone --help`. Also, if you want to see the help for a specific command, you can run `backbone <command> --help`.

Note: all commands need a connection string and a database provider. These can be supplied in two ways:

-   via command options (see `backbone --help`)
-   via the environment variables `Database__Provider` and `Database__ConnectionString`
