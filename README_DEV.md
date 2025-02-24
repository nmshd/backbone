# How to run the Backbone local

(only first time) docker volume create azure-storage-emulator-volume

(only first time) docker volume create postgres-volume

docker compose -f docker-compose/docker-compose.yml up

(only first time) dotnet run --project ./Applications/DatabaseMigrator/src/DatabaseMigrator/DatabaseMigrator.csproj

dotnet run --project ./Applications/AdminApi/src/AdminApi/AdminApi.csproj

// generate "test data"
dotnet run --project ./Applications/ConsumerApi/src/ConsumerApi.csproj

dotnet run --project ./Applications/EventHandlerService/src/EventHandlerService/EventHandlerService.csproj

Empty queue:
