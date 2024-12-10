#!/bin/bash

# Parameters with default values (can be overridden by arguments)
HOSTNAME=${1:-"host.docker.internal"}
USERNAME=${2:-"postgres"}
PASSWORD=${3:-"admin"}
DBNAME=${4:-"enmeshed"}
DUMPFILE=${5:-"enmeshed.pg"}

docker run --rm -v "$(pwd)/dump-files:/dump" --env PGPASSWORD=$PASSWORD postgres pg_dump -h $HOSTNAME -U $USERNAME $DBNAME -f /dump/$DUMPFILE

if [ $? -ne 0 ]; then
    echo "Error: Database export to $DUMPFILE failed."
    exit 1
fi

if [ ! -f "$(pwd)/dump-files/$DUMPFILE" ]; then
    echo "Snapshot file not found in the 'snapshots' folder."
    exit 1
fi

echo "Database export to $DUMPFILE successful."
