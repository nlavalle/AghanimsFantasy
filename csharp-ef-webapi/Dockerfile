# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
ARG configuration=Production

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore csharp-ef-webapi.csproj

# copy everything else and build app
COPY . .
RUN dotnet publish csharp-ef-webapi.csproj -c $configuration -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 5001
ENV ASPNETCORE_URLS=https://+:5001
ENTRYPOINT ["dotnet", "csharp-ef-webapi.dll"]
