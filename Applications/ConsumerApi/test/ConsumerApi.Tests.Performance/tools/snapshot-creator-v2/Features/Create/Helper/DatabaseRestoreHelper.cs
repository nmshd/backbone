using System.Diagnostics;
using Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Shared.Models;
using Microsoft.Extensions.Logging;

namespace Backbone.ConsumerApi.Tests.Performance.SnapshotCreator.V2.Features.Create.Helper;

public class DatabaseRestoreHelper(ILogger<DatabaseRestoreHelper> logger) : IDatabaseRestoreHelper
{
    private const string HOSTNAME = "host.docker.internal";
    private const string USERNAME = "postgres";
    private const string PASSWORD = "admin";
    private const string DB_NAME = "enmeshed";
    private const string CONTAINER_NAME = "tmp-postgres-container";

    public async Task<StatusMessage> RestoreCleanDatabase(string scriptPath)
    {
        try
        {
            var createContainerResult = CreatePostgresDockerContainer(CONTAINER_NAME, PASSWORD);

            if (!createContainerResult.Status)
            {
                throw new InvalidOperationException(createContainerResult.Message, createContainerResult.Exception);
            }

            logger.LogInformation("Postgres container created successfully: {Message}", createContainerResult.Message);


            var checkForOpenConnectionsResult = await CheckForOpenConnections(CONTAINER_NAME, HOSTNAME, USERNAME, PASSWORD, DB_NAME);

            if (checkForOpenConnectionsResult.Status)
            {
                logger.LogInformation("Open connections checked successfully: {Message}", checkForOpenConnectionsResult.Message);

                var forceDropOpenConnectionsResult = ForceDropOpenConnections(CONTAINER_NAME, HOSTNAME, USERNAME, PASSWORD, DB_NAME);

                if (!forceDropOpenConnectionsResult.Status)
                {
                    throw new InvalidOperationException(forceDropOpenConnectionsResult.Message, forceDropOpenConnectionsResult.Exception);
                }

                logger.LogInformation("Open connections terminated successfully: {Message}", forceDropOpenConnectionsResult.Message);
            }


            var executeScriptResult = ExecuteScript(scriptPath);

            if (!executeScriptResult.Status)
            {
                throw new InvalidOperationException(executeScriptResult.Message, executeScriptResult.Exception);
            }

            logger.LogInformation("Script executed successfully: {Message}", executeScriptResult.Message);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message, e);
        }

        return new StatusMessage(true, "Database restored successfully");
    }

    private StatusMessage CreatePostgresDockerContainer(string containerName, string password)
    {
        try
        {
            // Check if the container is already running
            var checkCommand = $"docker ps -q -f name={containerName}";
            var checkPsi = new ProcessStartInfo("cmd", $"/c {checkCommand}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var checkProcess = Process.Start(checkPsi);
            var checkOutput = checkProcess!.StandardOutput.ReadToEnd();
            var checkError = checkProcess.StandardError.ReadToEnd();
            checkProcess.WaitForExit();

            if (!string.IsNullOrEmpty(checkError))
            {
                return new StatusMessage(false, checkError);
            }

            if (!string.IsNullOrEmpty(checkOutput.Trim()))
            {
                return new StatusMessage(true, "Postgres container already running");
            }

            // Create the container
            var command = $"docker run -d --rm --name {containerName} -e POSTGRES_PASSWORD=\"{password}\" postgres";

            var psi = new ProcessStartInfo("cmd", $"/c {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            var createOutput = process!.StandardOutput.ReadToEnd();
            var createError = process.StandardError.ReadToEnd();

            process.WaitForExit();
            var isContainerCreated = !string.IsNullOrEmpty(createOutput.Trim());
            return isContainerCreated ? new StatusMessage(true, createOutput) : new StatusMessage(false, createError);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message, e);
        }
    }

    private static async Task<StatusMessage> CheckForOpenConnections(string containerName, string hostname, string username, string password, string dbName)
    {
        try
        {
            var query = $"SELECT pid FROM pg_stat_activity WHERE datname = '{dbName}';";
            var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} postgres -c \"{query}\"";

            var psi = new ProcessStartInfo("cmd", $"/c {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            var output = await process!.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();

            var hasOpenConnections = !output.Trim().Contains("(0 rows)", StringComparison.OrdinalIgnoreCase);

            return hasOpenConnections
                ? new StatusMessage(true, $"output: {output}. {(!string.IsNullOrWhiteSpace(error) ? $"error:{error}" : string.Empty)}")
                : new StatusMessage(false, $"No open connections found. {(!string.IsNullOrWhiteSpace(error) ? $"error:{error}" : string.Empty)}");
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message, e);
        }
    }

    private static StatusMessage ForceDropOpenConnections(string containerName, string hostname, string username, string password, string dbName)
    {
        try
        {
            var query = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{dbName}';";
            var command = $"docker exec --env PGPASSWORD=\"{password}\" {containerName} psql -h {hostname} -U {username} postgres -c \"{query}\"";

            var psi = new ProcessStartInfo("cmd", $"/c {command}")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            var output = process!.StandardOutput.ReadToEnd();
            process.WaitForExit();

            var succeed = process.ExitCode == 0 && !string.IsNullOrEmpty(output.Trim());
            return succeed ? new StatusMessage(true, "Open connections terminated successfully") : new StatusMessage(false, output);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message, e);
        }
    }

    private StatusMessage ExecuteScript(string scriptPath)
    {
        try
        {
            const string command = "powershell";
            var arguments = $"-File {scriptPath}";

            var psi = new ProcessStartInfo(command, arguments)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            var output = process!.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return process.ExitCode == 0 ? new StatusMessage(true, $"Script executed successfully. Output: {output}") : new StatusMessage(false, output);
        }
        catch (Exception e)
        {
            return new StatusMessage(false, e.Message, e);
        }
    }
}
