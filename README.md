# TV Maze Api
This projects is a POC on how to cache the TvMaze api data using latest ASP.NET Core version (6.0 at the moment) and uses latest version of EF Core (6.0 at the moment) to persist data into a local Sql Server db used as a cache.

The solution has a baground job that fetches data from the upstream api and exposes a controller to expose the cached data to the outside world.

>In order to make query faster, we use the new EF Core compiled model feature, and to make Json serialization faster I make use of the new System.Text.Json source generators.

## How to run
1. make sure you have the latest version of .NET (6.0) or download it from [here](https://get.dot.net)
2. run `dotnet build` in the main folder, the one that contains the solution
3. cd into the `src/TvMaze` folder
4. add the Sql Server connection string user secret via this command `dotnet user-secret add ` 
5. install **dotnet ef** global tool via `dotnet tool install --global dotnet-ef` or update it, more info [here](https://docs.microsoft.com/en-us/ef/core/cli/dotnet#installing-the-tools)
6. create the application database via `dotnet ef database update` 
7. run `dotnet run` from the web project folder (i.e. `src/TvMaze`)
8. navigate to `https://localhost:7074` to consume the api via the swagger-ui interface

## How to run tests
1. Make sure you have docker installed for running integration tests or install it from [here](https://docs.docker.com/get-docker/)
2. run `dotnet test` in the main folder, the one that contains the solution
