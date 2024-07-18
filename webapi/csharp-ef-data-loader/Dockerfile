# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
ARG configuration=Production

# copy csproj and restore as distinct layers
COPY ./csharp-ef-data-loader/csharp-ef-data-loader.csproj ./csharp-ef-data-loader/csharp-ef-data-loader.csproj
# copy dependent data access library
COPY ./data-access-library/data-access-library.csproj ./data-access-library/data-access-library.csproj
RUN dotnet restore ./csharp-ef-data-loader/csharp-ef-data-loader.csproj

# copy everything else and build app
COPY ./csharp-ef-data-loader/ ./csharp-ef-data-loader/
COPY ./data-access-library/ ./data-access-library/
RUN dotnet publish ./csharp-ef-data-loader/csharp-ef-data-loader.csproj -c $configuration -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "csharp-ef-data-loader.dll"]
