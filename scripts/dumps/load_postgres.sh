set -x
set -e

Hostname="host.docker.internal"
Username="postgres"
Password="admin"
DbName="enmeshed"
DumpFile="enmeshed.pg"

ContainerName="postgres-dump-loader"

docker run --name $ContainerName --rm -e POSTGRES_PASSWORD=$Password -d -v ./dump-files:/dump-files postgres

# Drop database if exists
docker exec --env PGPASSWORD=$Password -it $ContainerName dropdb --if-exists -h $Hostname -U $Username $DbName

# Create database
docker exec --env PGPASSWORD=$Password -it $ContainerName psql -h $Hostname -U $Username postgres -c "CREATE DATABASE $DbName WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8'"

# Alter database owner
docker exec --env PGPASSWORD=$Password -it $ContainerName psql -h $Hostname -U $Username postgres -c "ALTER DATABASE $DbName OWNER TO $Username;"

# Load dump file
docker exec --env PGPASSWORD=$Password -it $ContainerName psql -h $Hostname -U $Username $DbName -f /dump-files/$DumpFile

docker stop $ContainerName
