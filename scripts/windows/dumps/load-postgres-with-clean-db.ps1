# Parameters with default values (can be overridden by arguments)
param (
    [string]$Hostname = "host.docker.internal",
    [string]$Username = "postgres",
    [string]$Password = "admin",
    [string]$DbName = "enmeshed",
    [string]$DumpFile = "clean-db.rg"
)

$ContainerName = "tmp-postgres-container"

docker run -d --rm --name $ContainerName -v "$PSScriptRoot\dump-files:/dump" -e POSTGRES_PASSWORD="admin" postgres

docker exec --env PGPASSWORD=$Password -it $containerName psql -h $Hostname -U $Username postgres -c "DROP DATABASE IF EXISTS $DbName"
docker exec --env PGPASSWORD=$Password -it $containerName psql -h $Hostname -U $Username postgres -c "CREATE DATABASE $DbName"
docker exec --env PGPASSWORD=$Password -it $containerName psql -h $Hostname -U $Username postgres -c "ALTER DATABASE $DbName OWNER TO $Username;"
docker exec --env PGPASSWORD=$Password -it $containerName psql -h $Hostname -U $Username $DbName -f /dump/$DumpFile

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database import successful."
}

docker stop $ContainerName
