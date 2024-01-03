This guide provides a reference to C# Entity Framework Web APIs in ASP.NET Core
https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-6.0&tabs=visual-studio-code
The connection string to the postgres database is in the appsettings.json, but it's expecting the user/pass as env variables SQL_USER and SQL_PASSWORD

### Container setup
SSL cert bullshit
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx --format pem -np
dotnet dev-certs https --trust

### Local Development
(Powershell)
docker build . -t aghanims-wager-webapi
docker run -p 5001:5001 -e ASPNETCORE_HTTPS_PORT=5001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="<CREDENTIAL_PLACEHOLDER>" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v ${PWD}\dev-cert:/https/ -e SQL_USER=<sqluser> -e SQL_PASSWORD=<sqluser>  aghanims-wager-webapi

### Adding new controllers
Create a model of what you need then
` dotnet aspnet-codegenerator controller -name BalanceLedgerController -async -api -m BalanceLedger -dc AghanimsWagerContext -outDir Controllers`

### Adding new migrations
`dotnet ef migrations add <MigrationName> --context AghanimsWagerContext --output-dir Migrations`
We don't apply the ef database update right now because the database existed a long time before EF, and we don't want to blow up the whole thing rerunning all of the migrations