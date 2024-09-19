# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "enmeshed.pg"
)

$ContainerName = "tmp-postgres-container"

# Run a PostgreSQL container
docker run --name $ContainerName -e POSTGRES_PASSWORD="admin" -d postgres

# Perform the dump
docker exec --env PGPASSWORD=$Password -it $ContainerName pg_dump -h $Hostname -U $Username $DbName -f /tmp/$DumpFile

# Copy the dump file to the host
docker cp "${ContainerName}:/tmp/$DumpFile" "./dump-files/$DumpFile"

# Stop and remove the container
docker stop $ContainerName
docker rm $ContainerName

Write-Host "Database dump completed and saved to ./dump-files/$DumpFile"
