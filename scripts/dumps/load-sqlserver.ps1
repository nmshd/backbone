# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "SA",
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "enmeshed.bacpac", # Use .bacpac as extension
    [string]$Password = "Passw0rd"
)

$ContainerName = "tmp-mssql-server"
$HostDumpDir = "./dump-files"  # Directory on the host where the .bacpac file is located
$ContainerDumpDir = "/tmp/df"  # Directory in the container where the .bacpac will be accessible
$volumeArg = ($PSScriptRoot + "\" + $HostDumpDir) + ":/" + $ContainerDumpDir;

# Ensure the dump file exists in the host directory
if (-not (Test-Path "$HostDumpDir/$DumpFile")) {
    Write-Host "Error: The BACPAC file $DumpFile does not exist in $HostDumpDir"
    exit 1
}

# Run a SQL Server container in detached mode (so we can copy the file into it)
docker run -d --name $ContainerName -v $volumeArg ormico/sqlpackage sleep infinity

# Drop the database if it exists
$DropDatabaseQuery = "IF DB_ID(N'$DbName') IS NOT NULL BEGIN ALTER DATABASE [$DbName] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [$DbName]; END"
docker exec -ti $ContainerName /opt/mssql-tools/bin/sqlcmd -S $Hostname -U $Username -P $Password -Q "$DropDatabaseQuery"

# Check if the database drop was successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Pre-existing database dropped successfully (if it existed)."
    
    # Run the sqlpackage import command inside the container
    docker exec -ti $ContainerName sqlpackage /Action:Import /TargetServerName:$Hostname /TargetDatabaseName:$DbName /SourceFile:$ContainerDumpDir/$DumpFile /TargetUser:$Username /TargetPassword:$Password /TargetTrustServerCertificate:True
 
    # Check if the import command was successful
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database import from BACPAC successful."
    }
    else {
        Write-Host "Error: Database import from BACPAC failed."
        exit 1
    }
}
else {
    Write-Host "Warning: Could not drop the pre-existing database. Cancelling."
}

# Clean up: stop and remove the container
docker stop $ContainerName
docker rm $ContainerName
