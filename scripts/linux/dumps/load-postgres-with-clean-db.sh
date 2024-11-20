#!/bin/bash

# Parameters with default values (can be overridden by arguments)
HOSTNAME=${1:-"host.docker.internal"}
USERNAME=${2:-"postgres"}
PASSWORD=${3:-"admin"}
DBNAME=${4:-"enmeshed"}
DUMPFILE=${5:-"clean-db.rg"}

CONTAINER_NAME="tmp-postgres-container"

# Run the Docker container
docker run -d --rm --name $CONTAINER_NAME -v "$(pwd)/dump-files:/dump" -e POSTGRES_PASSWORD=$PASSWORD postgres

# Drop the existing database if it exists
docker exec --env PGPASSWORD=$PASSWORD -it $CONTAINER_NAME psql -h $HOSTNAME -U $USERNAME postgres -c "DROP DATABASE IF EXISTS $DBNAME"
# Create a new database
docker exec --env PGPASSWORD=$PASSWORD -it $CONTAINER_NAME psql -h $HOSTNAME -U $USERNAME postgres -c "CREATE DATABASE $DBNAME"
# Change the owner of the database
docker exec --env PGPASSWORD=$PASSWORD -it $CONTAINER_NAME psql -h $HOSTNAME -U $USERNAME postgres -c "ALTER DATABASE $DBNAME OWNER TO $USERNAME;"
# Import the dump file into the new database
docker exec --env PGPASSWORD=$PASSWORD -it $CONTAINER_NAME psql -h $HOSTNAME -U $USERNAME $DBNAME -f /dump/$DUMPFILE

if [ $? -eq 0 ]; then
    echo "Database import successful."
fi

# Stop the Docker container
docker stop $CONTAINER_NAME
