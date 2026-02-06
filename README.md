## About
ICMarketsTest is a .NET 9 Web API that stores BlockCypher blockchain snapshots (ETH, DASH, BTC, LTC) in SQLite.
It exposes history and sync endpoints, and a small YARP gateway for basic security and traffic control.

## Endpoints
Swagger (API): `https://localhost:7220/swagger` (Development only)

`GET /api/blockchains` - list supported networks (use this first to get provider keys like `btc-main`)
`GET /api/blockchains/snapshots?network=btc-main&limit=50` - snapshot history, newest first
`POST /api/blockchains/sync` - sync one network, body: `{ "network": "btc-main" }`
`POST /api/blockchains/sync-all` - sync all supported networks
`GET /health` - API health check

Gateway: https://localhost:7260
`GET /health` - gateway health check
`GET /api/...` - same API routes proxied through gateway

## Run
Docker:
`docker compose -f docker/docker-compose.yml up --build`

API:
`dotnet run --project src/Api/ICMarketsTest.Api.csproj`

Gateway (optional):
`dotnet run --project src/Gateway/ICMarketsTest.Gateway.csproj`

Tests:
`dotnet test`

Note: when calling the gateway, include `X-Api-Key` from `src/Gateway/appsettings.json`. The API must be running for the gateway to forward requests.

## Features
Layering and DI
- Thin API.
- Layered architecture: `ICMarketsTest.Api` = API layer, `ICMarketsTest.Core` = application/CQRS layer, `ICMarketsTest.Infrastructure` = data/integration layer, `ICMarketsTest.Contracts` = shared contracts.
- DI composes controllers, handlers, stores, and clients.

CQRS approach (Core)
- Commands for writes, Queries for reads. ICMarketsTest.Core/Handlers can be a good entry point to understand it 
- Handlers orchestrate fetch, persistence, and mapping per request.

Persistence (Infrastructure)
- EF Core with SQLite via a DbContext.
- Repository + Unit of Work for write paths; store abstraction for snapshot access.

External data ingestion
- BlockCypher HTTP client with throttling configuration and HTTP codes handling

Mapping and contracts
- AutoMapper profiles for API-to-core and persistence-to-DTO mapping.

Gateway (optional)
- Reverse proxy entrypoint with API key check and basic throttling.
- Separate gateway tests validate auth/limit behavior.

Operational basics
- Health checks, CORS, and request logging wired into the pipeline.

Docker integration
