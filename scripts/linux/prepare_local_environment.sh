set -e

YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NO_COLOR='\033[0m'

read -p "Enter the Azure Notification Hub Connection String: " notificationHubConnectionString
read -p "Enter the Azure Storage Account Connection String: " storageAccountConnectionString
echo "${YELLOW}Paste the following lines into the configuration file of your shell:"
echo
echo "${CYAN}export ENMESHED_AZURE_NOTIFICATION_HUB_CONNECTION_STRING=${notificationHubConnectionString}"
echo "export ENMESHED_BLOB_STORAGE_CONNECTION_STRING=Server=${storageAccountConnectionString}${YELLOW}"
echo
read -p "Press Enter after you completed the manual step above..." x
echo -n "${NO_COLOR}"

ENMESHED_AZURE_NOTIFICATION_HUB_CONNECTION_STRING=$notificationHubConnectionString
ENMESHED_BLOB_STORAGE_CONNECTION_STRING=$storageAccountConnectionString

repoRoot=$(git rev-parse --show-toplevel)

echo "Creating Docker volumes..."
docker volume create mssql-server-volume > /dev/null 2>&1
echo "OK"

echo "Setting up the database..."
docker compose -f $repoRoot/docker-compose/docker-compose.yml up -d ms-sql-server  > /dev/null 2>&1
docker run -it --rm --network backbone --volume $repoRoot/setup-sqlserver.sql:/setup-sqlserver.sql --entrypoint /opt/mssql-tools/bin/sqlcmd mcr.microsoft.com/mssql-tools -S ms-sql-server -U sa -P Passw0rd -i /setup-sqlserver.sql > /dev/null 2>&1
docker compose -f $repoRoot/docker-compose/docker-compose.yml down > /dev/null 2>&1
echo "OK"
