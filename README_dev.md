# Prerequisites

1. Download and install [Docker Desktop](https://www.docker.com/products/docker-desktop)
2. Download and install the latest version of the [.NET SDK](https://dotnet.microsoft.com/en-us/download)

# How to run

1. Start the infrastructure by running the following command from the root directory of the repository:
    ```bash
    docker compose -f up ./docker-compose/docker-compose.yml -d
    ```
2. Run the application by running the following commands from the root directory of the repository (each in its own terminal):

    ```bash
    dotnet run --project ./Applications/AdminApi/src/AdminApi/AdminApi.csproj

    dotnet run --project ./Applications/ConsumerApi/src/ConsumerApi.csproj

    dotnet run --project ./Applications/EventHandlerService/src/EventHandlerService/EventHandlerService.csproj
    ```
