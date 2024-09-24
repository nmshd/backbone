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
Write-Host "Creating container $ContainerName for loading dump onto the database"
docker container run --name $ContainerName -e POSTGRES_PASSWORD="admin" -d postgres

# Copy the dump file to the host
Write-Host "Copying file onto the container"
docker container cp "./dump-files/$DumpFile" "${ContainerName}:/tmp/$DumpFile"

Write-Host "Creating the Database $DbName"
docker container exec --env PGPASSWORD=admin -it bkb-postgres dropdb -h $Hostname -U $Username $DbName
docker container exec --env PGPASSWORD=admin -it bkb-postgres psql -h $Hostname -U $Username postgres -c "CREATE DATABASE $DbName WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8'"
docker container exec --env PGPASSWORD=admin -it bkb-postgres psql -h $Hostname -U $Username postgres -c "ALTER DATABASE $DbName OWNER TO $Username;"
docker container exec --env PGPASSWORD=admin -it bkb-postgres psql -h $Hostname -U $Username $DbName -f /tmp/$DumpFile

# Stop and remove the container
docker container stop $ContainerName
docker container rm $ContainerName

Write-Host "Database dump completed and saved to ./dump-files/$DumpFile"
