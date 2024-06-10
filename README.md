# Aghanim's Fantasy

Work in progress, goal is to have a fantasy league setup primarily for the NADCL.

### Requirements
This project depends on three things:
1. A valid Steam API Key to make background calls to dota looking for league games and match details. You can request for a steamkey here: https://steamcommunity.com/dev/apikey
2. A discord bot client ID/secret, full information on creating a bot can be found here: https://discord.com/developers/docs/getting-started
3. A valid Steam account to log in and talk to the game coordinator, this is needed for replay parsing that we can't get via the API.

#### SSL Certificates
You'll need to create certificates with dev-certs so that the localhost will work on https
Msft says to use pfx but I think using a .pem and .key makes it easier to map to
`dotnet dev-certs https -ep .\ssl_certs\development.pem --format pem -np`
then:
`dotnet dev-certs https --trust`

Double check the .\csharp-ef-webapi\appsettings.Development.json and docker-compose.yaml, the Kestrel endpoints certificate path should point to /https/development.pem and docker-compose should be mapping .\ssl_certs\=.\https\

### How to use

Fill in the secrets for the `postgres.env-example` and `webapi.env-example` and rename them to `postgres.env` and `webapi.env`

`cd environments/development`
`docker-compose build postgres webapi spa`
`docker-compose up postgres webapi spa`

The intent for the docker compose is that if you want to work on the webapi, you can do a `docker-compose up postgres` and plug into the local postgres endpoint debugging webapi locally. And the equivalent for working with spa you can do `docker-compose up postgres webapi` then go into the quasar frontend and run `quasar dev` and hit your local endpoint.

### Depedencies
This project makes use of the SteamKit library from: https://github.com/SteamRE/SteamKit