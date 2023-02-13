
Write-Host "Setting environment variables..."
$AzureNotificationHubConnectionString = Read-Host -Prompt "Enter the Azure Notification Hub Connection String"
$AzureStorageAccountConnectionString = Read-Host -Prompt "Enter the Azure Storage Account Connection String"
[Environment]::SetEnvironmentVariable("ENMESHED_AZURE_NOTIFICATION_HUB_CONNECTION_STRING", $AzureNotificationHubConnectionString, "User")
[Environment]::SetEnvironmentVariable("ENMESHED_BLOB_STORAGE_CONNECTION_STRING", $AzureStorageAccountConnectionString, "User")
Write-Host "OK"

$repoRoot = git rev-parse --show-toplevel

Write-Host "Creating Docker volumes..."
docker volume create mssql-server-volume | Out-Null
Write-Host "OK"

Write-Host "Setting up the database..."
docker compose -f $repoRoot\docker-compose\docker-compose.yml up -d ms-sql-server  | Out-Null
docker run -it --rm --network backbone --volume $repoRoot\setup-sqlserver.sql:/setup-sqlserver.sql --entrypoint /opt/mssql-tools/bin/sqlcmd mcr.microsoft.com/mssql-tools -S ms-sql-server -U sa -P Passw0rd -i /setup-sqlserver.sql | Out-Null
docker compose -f $repoRoot\docker-compose\docker-compose.yml down | Out-Null
Write-Host "OK"
