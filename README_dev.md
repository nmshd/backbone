# Prerequisites

1. Download and install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Download and install the latest version of the [.NET SDK](https://dotnet.microsoft.com/en-us/download)
3. Use Docker Desktop to create Docker volumes for the database and the blob storage. You can do this via the Docker Desktop UI or by running the following commands:
    ```bash
    docker volume create --name=postgres-volume
    docker volume create --name=azure-storage-emulator-volume
    ```

# How to run

1. Start the infrastructure by running the following command from the root directory of the repository:
    ```bash
    docker compose -f  ./docker-compose/docker-compose.yml up -d
    ```
2. Run the application by running the following commands from the root directory of the repository (each in its own terminal):

    ```bash
    dotnet run --project ./Applications/AdminApi/src/AdminApi/AdminApi.csproj

    dotnet run --project ./Applications/ConsumerApi/src/ConsumerApi.csproj

    dotnet run --project ./Applications/EventHandlerService/src/EventHandlerService/EventHandlerService.csproj
    ```
