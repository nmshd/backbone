#!/bin/bash

# Check if unzip is installed, and install it if not
if ! command -v unzip &> /dev/null; then
    echo "'unzip' is not installed. Installing it now..."
    sudo apt update && sudo apt install -y unzip
fi
# Default values for parameters
SNAPSHOT_NAME=$1
HOSTNAME=${2:-"host.docker.internal"}
USERNAME=${3:-"postgres"}
PASSWORD=${4:-"admin"}
DB_NAME=${5:-"enmeshed"}

# Prompt for the snapshot name if not provided
if [ -z "$SNAPSHOT_NAME" ]; then
    read -p "Enter the snapshot name: " SNAPSHOT_NAME
fi

# Get the current script location
SCRIPT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
ROOT_PATH=$(realpath "$SCRIPT_PATH/../..")
SNAPSHOTS_FOLDER="$ROOT_PATH/snapshots"
SNAPSHOT_FILE_PATH="$SNAPSHOTS_FOLDER/$SNAPSHOT_NAME.zip"
REPO_ROOT=$(realpath "$SCRIPT_PATH/../../../../../..")

# Check if the file exists
echo "Snapshot file path: $SNAPSHOT_FILE_PATH"
if [ ! -f "$SNAPSHOT_FILE_PATH" ]; then
    echo "Snapshot file '$SNAPSHOT_NAME' not found in the 'snapshots' folder."
    exit 1
fi

# Extract the zip file
echo "Extracting '$SNAPSHOT_NAME'..."
if unzip -o "$SNAPSHOT_FILE_PATH" -d "$SNAPSHOTS_FOLDER"; then
    echo "Extraction complete. Files are in '$SNAPSHOTS_FOLDER'."
    mv -f "$SNAPSHOTS_FOLDER/$SNAPSHOT_NAME/enmeshed.pg" "$REPO_ROOT/scripts/dumps/dump-files/"
else
    echo "An error occurred during extraction."
    exit 1
fi

# Load Postgres
LOAD_POSTGRES="$REPO_ROOT/scripts/dumps/load_postgres.sh"
bash "$LOAD_POSTGRES" "$HOSTNAME" "$USERNAME" "$PASSWORD" "$DB_NAME" "enmeshed.pg"
