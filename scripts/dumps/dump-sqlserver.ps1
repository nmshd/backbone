# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "SA",
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "enmeshed.bacpac", # Use .bacpac as extension
    [string]$Password = "Passw0rd"
)

$ContainerName = "tmp-mssql-server"

# Run a SQL Server container (if not already running)
docker container run -ti --name $ContainerName ormico/sqlpackage sqlpackage /Action:Export /SourceServerName:$Hostname /SourceDatabaseName:$DbName /TargetFile:/tmp/$DumpFile /SourceUser:$Username /SourcePassword:$Password /SourceTrustServerCertificate:True

# Check if the export command was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Database export to BACPAC successful. Proceeding to copy the file..."
    
    # Copy the BACPAC file to the host
    docker cp "${ContainerName}:/tmp/$DumpFile" "./dump-files/$DumpFile"
    
    # Check if the file was successfully copied
    if (Test-Path "./dump-files/$DumpFile") {
        Write-Host "Database export completed and saved to ./dump-files/$DumpFile"
    }
    else {
        Write-Host "Error: Failed to copy the BACPAC file to the host."
    }
}
else {
    Write-Host "Error: Database export to BACPAC failed."
}

# Stop and remove the container
docker stop $ContainerName
docker rm $ContainerName
