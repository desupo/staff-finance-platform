# DevOps Notes

## Git Strategy

- protect `main`
- require pull requests
- require CI success before merge
- keep one work item per branch

## Suggested Work Items

1. `Loan application submission`
2. `Loan review workflow`
3. `Fund claim submission`
4. `Fund claim approval workflow`
5. `Notification integration`
6. `AKS deployment`
7. `App Service deployment`

## Azure Targets

### App Service

Good for:

- simpler service hosting
- lower operational complexity
- interview discussion around PaaS

### AKS

Good for:

- Kubernetes deployment patterns
- scaling, readiness probes, configuration separation
- stronger microservice operations story

## Required GitHub Secrets

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `AZURE_WEBAPP_PUBLISH_PROFILE_LOAN`
- `AZURE_WEBAPP_PUBLISH_PROFILE_CLAIMS`
- `AZURE_RESOURCE_GROUP`
- `AZURE_AKS_CLUSTER`
- `AZURE_ACR_NAME`

## Key Vault Convention

- Store Mongo connection string as Key Vault secret `ConnectionStrings--Mongo`
- Configure app setting `KeyVault__Uri` (or `KeyVault__Name`) per service in Azure
- Grant the app managed identity `Key Vault Secrets User` on the vault
- Keep local/debug values in `appsettings.Development.json` using `ConnectionStrings:Mongo`

## Notes

This repository ships both App Service and AKS deployment workflows so you can discuss tradeoffs in the interview rather than committing to only one hosting style.
