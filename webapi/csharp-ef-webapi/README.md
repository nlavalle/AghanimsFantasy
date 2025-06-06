### Container setup
SSL cert generation
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx --format pem -np
dotnet dev-certs https --trust

### Local Development
(Powershell)
docker build . -t aghanims-wager-webapi
docker run -p 5001:5001 -e ASPNETCORE_HTTPS_PORT=5001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="<CREDENTIAL_PLACEHOLDER>" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v ${PWD}\dev-cert:/https/ -e SQL_USER=<sqluser> -e SQL_PASSWORD=<sqluser>  aghanims-wager-webapi

### Adding new controllers
Create a model of what you need then
` dotnet aspnet-codegenerator controller -name BalanceLedgerController -async -api -m BalanceLedger -dc AghanimsFantasyContext -outDir Controllers`

### Adding new migrations
`dotnet ef migrations add <MigrationName> --context AghanimsFantasyContext --output-dir Migrations`
We don't apply the ef database update right now because the database existed a long time before EF, and we don't want to blow up the whole thing rerunning all of the migrations
`dotnet ef migrations script OldMigration NewMigration > ./Migrations/migration_up_NewMigration.sql`

### Removing migrations
The env variables in the connection strings will probably mess up the `dotnet ef migrations remove` command.
First export the Local `export ASPNETCORE_ENVIRONMENT=Local` or `$env:ASPNETCORE_ENVIRONMENT='Local'` on windows
Make sure `appsettings.Local.json` is defined then the command is:
`dotnet ef migrations remove --configuration Local`

### Notes
The connection string to the postgres database is in the appsettings.json, but it's expecting the user/pass as env variables SQL_USER and SQL_PASSWORD

### Depedencies
This project makes use of the SteamKit library from: https://github.com/SteamRE/SteamKit