
$repoRoot = git rev-parse --show-toplevel

Write-Host "Creating Docker volumes..."
docker volume create mssql-server-volume | Out-Null
Write-Host "OK"

Write-Host "Setting up the database..."
docker compose -f $repoRoot\docker-compose\docker-compose.yml up -d ms-sql-server  | Out-Null
docker run -it --rm --network backbone --volume $repoRoot\setup-sqlserver.sql:/setup-sqlserver.sql --entrypoint /opt/mssql-tools/bin/sqlcmd mcr.microsoft.com/mssql-tools -S ms-sql-server -U sa -P Passw0rd -i /setup-sqlserver.sql | Out-Null
docker compose -f $repoRoot\docker-compose\docker-compose.yml down | Out-Null
Write-Host "OK"
