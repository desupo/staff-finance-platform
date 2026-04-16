# Architecture Overview

## Service Boundaries

### Loan Service

Responsibilities:

- submit staff loan applications
- review, approve, and reject loan requests
- publish status-change notifications

### Fund Claims Service

Responsibilities:

- submit staff fund claims
- review, approve, and reject reimbursement requests
- publish status-change notifications

## Clean Architecture

Each service follows:

- `Domain`: aggregates, enums, invariants
- `Application`: use cases, ports, DTOs
- `Infrastructure`: EF Core Mongo/Cosmos implementation, service bus publisher
- `Api`: HTTP endpoints, dependency injection, configuration

## DDD Notes

- `LoanApplication` and `FundClaim` are aggregate roots
- status transitions are enforced inside the aggregate
- infrastructure depends inward; domain has no infrastructure dependency

## Data Store Strategy

Primary target:

- `Azure Cosmos DB for MongoDB vCore`

Development option:

- `MongoDB Atlas M0 free cluster`

Reasoning:

- MongoDB-compatible experience
- Azure-aligned deployment story
- EF Core support via MongoDB's official provider

## Event-Driven Design

- commands update aggregate state
- application services publish integration messages
- a Python Azure Function processes Service Bus messages for notifications
