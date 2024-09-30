# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "enmeshed.pg"
)

$ContainerName = "tmp-postgres-container"

docker container run --rm --name $ContainerName -v $PSScriptRoot\dump-files:/dump -e POSTGRES_PASSWORD="admin" -d postgres

docker container exec --env PGPASSWORD=admin -it $containerName dropdb --if-exists -h $Hostname -U $Username $DbName
docker container exec --env PGPASSWORD=admin -it $containerName psql -h $Hostname -U $Username postgres -c "CREATE DATABASE $DbName WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8'"
docker container exec --env PGPASSWORD=admin -it $containerName psql -h $Hostname -U $Username postgres -c "ALTER DATABASE $DbName OWNER TO $Username;"
docker container exec --env PGPASSWORD=admin -it $containerName psql -h $Hostname -U $Username $DbName -f /dump/$DumpFile

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database dump loaded successfully from $DumpFile."
}

docker container stop $ContainerName
