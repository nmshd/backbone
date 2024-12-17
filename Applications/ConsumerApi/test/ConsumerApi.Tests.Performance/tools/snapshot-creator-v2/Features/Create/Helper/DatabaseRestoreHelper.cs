using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public class DatabaseRestoreHelper(ILogger<DatabaseRestoreHelper> logger, IProcessHelper processHelper) : IDatabaseRestoreHelper
{
    private const string HOSTNAME = "host.docker.internal";
    private const string USERNAME = "postgres";
    private const string PASSWORD = "admin";
    private const string DB_NAME = "enmeshed";
    private const string CONTAINER_NAME = "tmp-postgres-container";
    private const string CLEAN_DB_NAME = "clean-db.rg";
    private const string SNAPHOT_DB_NAME = "snapshot-db.rg";

    private static bool QueryUserConsent()
    {
        Console.WriteLine("Do you want to drop db and create a clean-db before snapshot is created(default: y)? (y/n)");
        var input = Console.ReadKey().KeyChar;

        if (input is not ('y' or 'n'))
        {
            input = 'y';
        }

        return input == 'y';
    }

    public async Task<DatabaseResult> RestoreCleanDatabase()
    {
        try
        {
            var createContainerResult = await CreateTemporaryPostgresDockerContainer(CONTAINER_NAME, PASSWORD);

            if (!createContainerResult.Status)
            {
                throw new InvalidOperationException(createContainerResult.Message, createContainerResult.Exception);
            }

            logger.LogInformation("Postgres container created successfully: {Message}", createContainerResult.Message);

            var checkForOpenConnectionsResult = await CheckForOpenConnections(CONTAINER_NAME, HOSTNAME, USERNAME, PASSWORD, DB_NAME);

            switch (checkForOpenConnectionsResult.Status)
            {
                case false when checkForOpenConnectionsResult.IsError:
                    return new DatabaseResult(false, checkForOpenConnectionsResult.Message);
                case true:
                {
                    logger.LogInformation("Open connections checked successfully: {Message}", checkForOpenConnectionsResult.Message);

                    var consent = QueryUserConsent();

                    if (!consent)
                    {
                        return new DatabaseResult(true, "User chose not to drop db and create a clean-db");
                    }

                    var forceDropOpenConnectionsResult = await ForceDropOpenConnections(CONTAINER_NAME, HOSTNAME, USERNAME, PASSWORD, DB_NAME);

                    if (!forceDropOpenConnectionsResult.Status)
                    {
                        throw new InvalidOperationException(forceDropOpenConnectionsResult.Message);
                    }

                    logger.LogInformation("Open connections terminated successfully: {Message}", forceDropOpenConnectionsResult.Message);
                    break;
                }
            }


            var result = await DropDatabase(CONTAINER_NAME, PASSWORD, HOSTNAME, USERNAME, DB_NAME);

            if (!result.Status)
            {
                throw new InvalidOperationException(result.Message);
            }

            logger.LogInformation("{Message}", result.Message);

            result = await CreateDatabase(CONTAINER_NAME, PASSWORD, HOSTNAME, USERNAME, DB_NAME);

            if (!result.Status)
            {
                throw new InvalidOperationException(result.Message);
            }

            logger.LogInformation("{Message}", result.Message);

            result = await AlterDatabase(CONTAINER_NAME, PASSWORD, HOSTNAME, USERNAME, DB_NAME);

            if (!result.Status)
            {
                throw new InvalidOperationException(result.Message);
            }

            logger.LogInformation("{Message}", result.Message);

            result = await RestoreDatabase(CONTAINER_NAME, PASSWORD, HOSTNAME, USERNAME, DB_NAME, CLEAN_DB_NAME);

            if (!result.Status)
            {
                throw new InvalidOperationException(result.Message);
            }

            logger.LogInformation("{Message}", result.Message);
        }
        catch (Exception e)
        {
            return new DatabaseResult(false, e.Message, Exception: e);
        }
        finally
        {
            await StopTemporaryPostgresDockerContainer(CONTAINER_NAME);
        }

        return new DatabaseResult(true, "Database restored successfully");
    }

    public async Task<DatabaseResult> BackupDatabase(string outputDirName)
    {
        try
        {
            var createContainerResult = await CreateTemporaryPostgresDockerContainer(CONTAINER_NAME, PASSWORD, forceRecreate: true, outputDirName);

            if (!createContainerResult.Status)
            {
                throw new InvalidOperationException(createContainerResult.Message, createContainerResult.Exception);
            }

            logger.LogInformation("Postgres container created successfully: {Message}", createContainerResult.Message);

            var result = await DumpDatabase(CONTAINER_NAME, PASSWORD, HOSTNAME, USERNAME, DB_NAME, SNAPHOT_DB_NAME);

            if (!result.Status)
            {
                throw new InvalidOperationException(result.Message);
            }
        }
        catch (Exception e)
        {
            return new DatabaseResult(false, e.Message, Exception: e);
        }
        finally
        {
            await StopTemporaryPostgresDockerContainer(CONTAINER_NAME);
        }


        return new DatabaseResult(true, "Database backup completed successfully");
    }


    private async Task<DatabaseResult> DropDatabase(string containerName, string password, string hostname, string username, string dbname)
    {
        var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} postgres -c \"DROP DATABASE IF EXISTS {dbname}\"";
        return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                            !string.IsNullOrWhiteSpace(processParams.Output) &&
                                                                            !processParams.HasError);
    }

    private async Task<DatabaseResult> DumpDatabase(string containerName, string password, string hostname, string username, string dbname, string backupFile)
    {
        var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} pg_dump -h {hostname} -U {username} {dbname} -f /dump/{backupFile}";
        return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                            !processParams.HasError);
    }

    private async Task<DatabaseResult> CreateDatabase(string containerName, string password, string hostname, string username, string dbname)
    {
        var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} postgres -c \"CREATE DATABASE {dbname}\"";
        return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                            !string.IsNullOrWhiteSpace(processParams.Output) &&
                                                                            !processParams.HasError);
    }

    private async Task<DatabaseResult> AlterDatabase(string containerName, string password, string hostname, string username, string dbname)
    {
        var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} postgres -c \"ALTER DATABASE {dbname} OWNER TO {username};\"";
        return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                            !string.IsNullOrWhiteSpace(processParams.Output) &&
                                                                            !processParams.HasError);
    }

    private async Task<DatabaseResult> RestoreDatabase(string containerName, string password, string hostname, string username, string dbname, string restoreFile)
    {
        var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} {dbname} -f /dump/{restoreFile}";
        return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                            !string.IsNullOrWhiteSpace(processParams.Output) &&
                                                                            !processParams.HasError);
    }

    private async Task<DatabaseResult> CreateTemporaryPostgresDockerContainer(string containerName, string password, bool forceRecreate = false, string? outputDirectory = null)
    {
        try
        {
            if (forceRecreate)
            {
                await StopTemporaryPostgresDockerContainer(containerName);
            }
            else
            {
                var result = await IsTemporaryContainerRunning(containerName);
                if (result.Status) return result;

                if (result.IsError)
                {
                    return new DatabaseResult(false, result.Message);
                }
            }

            var directory = outputDirectory ?? Path.Combine(AppContext.BaseDirectory, @"Config\Database\dump-files");
            var command = $"docker run -d --rm --name {containerName} -v \"{directory}:/dump\" -e POSTGRES_PASSWORD=\"{password}\" postgres";
            // docker run -d --rm --name $ContainerName -v "$PSScriptRoot\dump-files:/dump" -e POSTGRES_PASSWORD="admin" postgres
            return await processHelper.ExecuteProcess(command, processParams => !string.IsNullOrEmpty(processParams.Output?.Trim()));
        }
        catch (Exception e)
        {
            return new DatabaseResult(false, e.Message, Exception: e);
        }
    }

    private async Task<DatabaseResult> IsTemporaryContainerRunning(string containerName)
    {
        try
        {
            var command = $"docker ps -q -f name={containerName}";
            return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                                !string.IsNullOrWhiteSpace(processParams.Output) &&
                                                                                !processParams.HasError);
        }
        catch (Exception e)
        {
            return new DatabaseResult(false, e.Message, Exception: e);
        }
    }

    private async Task StopTemporaryPostgresDockerContainer(string containerName)
    {
        var command = $"docker stop {containerName}";

        var result = await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                                  !processParams.HasError);

        if (result.Status)
        {
            logger.LogInformation("Temporary postgres container stopped successfully: {Message}", result.Message);
        }
        else
        {
            logger.LogError("Error stopping the temporary postgres container: {Message}", result.Message);
        }
    }

    private async Task<DatabaseResult> CheckForOpenConnections(string containerName, string hostname, string username, string password, string dbName)
    {
        try
        {
            var databaseQuery = $"SELECT pid FROM pg_stat_activity WHERE datname = '{dbName}';";
            var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} postgres -c \"{databaseQuery}\"";

            return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                                processParams.Output != null &&
                                                                                !processParams.Output.Trim().Contains("(0 rows)", StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception e)
        {
            return new DatabaseResult(false, e.Message, Exception: e);
        }
    }

    private async Task<DatabaseResult> ForceDropOpenConnections(string containerName, string hostname, string username, string password, string dbName)
    {
        try
        {
            var query = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{dbName}';";
            var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} postgres -c \"{query}\"";

            return await processHelper.ExecuteProcess(command, processParams => processParams.Process.ExitCode == 0 &&
                                                                                !string.IsNullOrEmpty(processParams.Output?.Trim()) &&
                                                                                !processParams.HasError);
        }
        catch (Exception e)
        {
            return new DatabaseResult(false, e.Message, Exception: e);
        }
    }
}
