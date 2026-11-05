#!/bin/bash
set -euo pipefail

compose=(docker compose "$@")

# Work around intermittent SQLPAL startup crashes until SQL Server 2025 CU12.
# https://github.com/microsoft/mssql-docker/issues/974
# Compose can fail while Docker is restarting a container, so retry explicitly.
for attempt in 1 2 3; do
    echo "Starting database (attempt $attempt/3)"
    if "${compose[@]}" up --no-build --force-recreate --wait --wait-timeout 120 -d database; then
        "${compose[@]}" up --no-build --wait -d
        exit 0
    fi

    "${compose[@]}" logs database
done

echo "Database failed to become healthy after 3 attempts." >&2
exit 1
