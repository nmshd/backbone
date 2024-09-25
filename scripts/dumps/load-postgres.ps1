# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "enmeshed.pg"
)

$ContainerName = "tmp-postgres-container"

Write-Host ($PSScriptRoot + "\dump-files")

$volumeArg = ($PSScriptRoot + "\dump-files") + ":/tmp/df";
Write-Host $volumeArg

# Run a PostgreSQL container
Write-Host "Creating container $ContainerName for loading dump onto the database"
docker container run --name $ContainerName -v $volumeArg -e POSTGRES_PASSWORD="admin" -d postgres

Write-Host "Creating the Database $DbName"
docker container exec --env PGPASSWORD=admin -it $containerName dropdb --if-exists -h $Hostname -U $Username $DbName
docker container exec --env PGPASSWORD=admin -it $containerName psql -h $Hostname -U $Username postgres -c "CREATE DATABASE $DbName WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8'"
docker container exec --env PGPASSWORD=admin -it $containerName psql -h $Hostname -U $Username postgres -c "ALTER DATABASE $DbName OWNER TO $Username;"
docker container exec --env PGPASSWORD=admin -it $containerName psql -h $Hostname -U $Username $DbName -f /tmp/df/$DumpFile

# Stop and remove the container
docker container stop $ContainerName
docker container rm $ContainerName

Write-Host "Database dump load completed."
