# Staff Finance Platform

Interview-grade sample platform built with:

- `.NET 8`
- `EF Core + MongoDB.EntityFrameworkCore`
- `Azure Cosmos DB for MongoDB vCore` or `MongoDB Atlas free tier`
- `Clean Architecture`
- `DDD`
- `SOLID`
- `TDD`
- `GitHub Actions`
- `Azure App Service / AKS deployment paths`

## Bounded Contexts

- `LoanService`: staff loan application lifecycle
- `FundClaimsService`: staff reimbursement and fund claims lifecycle

## Solution Layout

- `src/BuildingBlocks`: lightweight shared abstractions
- `src/Services/*`: each microservice owns its domain, application, infrastructure, and API
- `tests`: domain and architecture tests
- `functions`: Python Azure Function for asynchronous notifications
- `.github/workflows`: CI and deployment pipelines
- `infra`: AKS manifests and deployment assets
- `docs`: architecture and delivery guidance

## Why Cosmos DB for MongoDB

This repo is intentionally designed for `Azure Cosmos DB for MongoDB vCore` so you get:

- MongoDB wire protocol compatibility
- Azure hosting alignment for the interview story
- EF Core support through MongoDB's official EF Core provider

For cheap local cloud testing, you can also point the services at a `MongoDB Atlas M0` free cluster.

## Quick Start

1. Create either:
   - a `MongoDB Atlas` free cluster, or
   - an `Azure Cosmos DB for MongoDB vCore` cluster
2. Copy the connection string into each API project's `appsettings.Development.json` or user secrets.
   The checked-in development config defaults to `mongodb://localhost:27017` so the APIs can also run against a local Mongo container.
3. Restore and run:

```powershell
dotnet restore
dotnet test
dotnet run --project .\src\Services\LoanService\LoanService.Api
dotnet run --project .\src\Services\FundClaimsService\FundClaimsService.Api
```

## Secret Management Standard

- Standard key: `ConnectionStrings:Mongo`
- Local development: set `ConnectionStrings:Mongo` in `appsettings.Development.json` (already set to localhost by default)
- Production: load from Azure Key Vault using managed identity

For Key Vault, create the secret using double dashes:

- `ConnectionStrings--Mongo` = your production Mongo/Cosmos connection string

Each API can enable Key Vault by setting either:

- `KeyVault:Uri` (recommended), or
- `KeyVault:Name` (the app builds `https://<name>.vault.azure.net/`)

If Key Vault is not configured or unavailable (for local/debug), the app falls back to local configuration.

## Branching Model

- `main`: protected integration branch
- `feature/<work-item-id>-<short-description>`: one branch per user story or task
- `release/<version>`: optional release hardening branch
- `hotfix/<issue-id>`: urgent production fixes

Examples:

- `feature/101-loan-application-submission`
- `feature/102-fund-claim-approval`

## Eventing and Notifications

- APIs publish business notifications to `Azure Service Bus`
- `Python Azure Function` consumes messages asynchronously
- `Notification Hubs` is included for push notifications
- Email should be sent by `Azure Communication Services Email` or `SendGrid`, not Notification Hubs

That last point matters technically: `Azure Notification Hubs` is not an email delivery product.

## References

- MongoDB EF Core Provider docs: https://www.mongodb.com/docs/entity-framework/current/
- Azure Cosmos DB for MongoDB docs: https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/
