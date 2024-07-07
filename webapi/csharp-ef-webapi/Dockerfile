# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
ARG configuration=Production

# copy csproj and restore as distinct layers
COPY ./csharp-ef-webapi/csharp-ef-webapi.csproj ./csharp-ef-webapi/csharp-ef-webapi.csproj
# copy dependent data access library
COPY ./data-access-library/data-access-library.csproj ./data-access-library/data-access-library.csproj
RUN dotnet restore ./csharp-ef-webapi/csharp-ef-webapi.csproj

# copy everything else and build app
COPY ./csharp-ef-webapi/ ./csharp-ef-webapi/
COPY ./data-access-library/ ./data-access-library/
RUN dotnet publish ./csharp-ef-webapi/csharp-ef-webapi.csproj -c $configuration -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5001
ENV ASPNETCORE_URLS=https://+:5001
ENTRYPOINT ["dotnet", "csharp-ef-webapi.dll"]
