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
Write-Host "Creating container $ContainerName for pg_dump execution"
docker container run --name $ContainerName -e POSTGRES_PASSWORD="admin" -d postgres

# Perform the dump
docker container exec --env PGPASSWORD=$Password -it $ContainerName pg_dump -h $Hostname -U $Username $DbName -f /tmp/$DumpFile

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database export to $DumpFile successful. Proceeding to copy the file..."
    
    # Copy the dump file to the host
    docker container cp "${ContainerName}:/tmp/$DumpFile" "./dump-files/$DumpFile"

    # Check if the file was successfully copied
    if (Test-Path "./dump-files/$DumpFile") {
        Write-Host "Database export completed and saved to ./dump-files/$DumpFile"
    }
    else {
        Write-Host "Error: Failed to copy the $DumpFile file to the host."
    }
}
else {
    Write-Host "Error: Database export to $DumpFile failed."
}


# Stop and remove the container
docker container stop $ContainerName
docker container rm $ContainerName

Write-Host "Container $ContainerName stopped and removed"
