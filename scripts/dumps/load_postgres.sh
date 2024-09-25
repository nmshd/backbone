#!/bin/bash

# Parameters with default values (can be overridden by arguments)
HOSTNAME=${1:-"host.docker.internal"}
USERNAME=${2:-"postgres"}
PASSWORD=${3:-"admin"}
DB_NAME=${4:-"enmeshed"}
DUMP_FILE=${5:-"enmeshed.pg"}

CONTAINER_NAME="tmp-postgres-container"

# Run a PostgreSQL container
echo "Creating container $CONTAINER_NAME for loading dump onto the database"
VOLUME_ARG="$(dirname "${BASH_SOURCE[0]}")/dump-files:/tmp/df"
docker container run --name "$CONTAINER_NAME" -v "$VOLUME_ARG" -e POSTGRES_PASSWORD="admin" -d postgres

echo "Creating the Database $DB_NAME"
docker container exec --env PGPASSWORD=admin -i "$CONTAINER_NAME" dropdb --if-exists -h "$HOSTNAME" -U "$USERNAME" "$DB_NAME"
docker container exec --env PGPASSWORD=admin -i "$CONTAINER_NAME" psql -h "$HOSTNAME" -U "$USERNAME" postgres -c "CREATE DATABASE $DB_NAME WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8'"
docker container exec --env PGPASSWORD=admin -i "$CONTAINER_NAME" psql -h "$HOSTNAME" -U "$USERNAME" postgres -c "ALTER DATABASE $DB_NAME OWNER TO $USERNAME;"
docker container exec --env PGPASSWORD=admin -i "$CONTAINER_NAME" psql -h "$HOSTNAME" -U "$USERNAME" "$DB_NAME" -f "/tmp/df/$DUMP_FILE"

# Stop and remove the container
docker container stop "$CONTAINER_NAME"
docker container rm "$CONTAINER_NAME"

echo "Database dump load completed."
