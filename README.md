# WorkforceApp

## Overview
WorkforceApp is a .NET solution for managing employees and skills.

It includes:
- A backend API for employees, skills, and skill assignment.
- A Blazor frontend for viewing/filtering employees and managing skills.
- An Aspire AppHost that orchestrates local dependencies.

## Repository Structure
- `src/backend/Workforce.API`: HTTP API endpoints.
- `src/backend/Workforce.Domain`: Domain model and value objects.
- `src/backend/Workforce.Infrastructure`: EF Core data access.
- `src/backend/Workforce.Migrator`: Database migration/seeding worker.
- `src/frontend/Workforce.Web`: Blazor host app.
- `src/frontend/Workforce.Web/Workforce.Web.Client`: Blazor client UI.
- `src/aspire/Workforce.AppHost`: Aspire orchestration entry point.
- `tests/Workforce.UnitTests`: Unit tests.

## Prerequisites
- .NET SDK (matching `global.json`; current project targets .NET 11 preview).
- Docker Desktop (required by Aspire-managed containers, e.g. SQL Server and Dex).
- Git.

## Setup Instructions
1. Extract the submitted archive and open a terminal in the project root:

```bash
cd WorkforceApp
```

2. Restore dependencies and verify build:

```bash
dotnet restore
dotnet build
```

3. Start the full local stack with Aspire AppHost:

```bash
dotnet run --project src/aspire/Workforce.AppHost/Workforce.AppHost.csproj
```

4. Open the Aspire dashboard URL printed in the terminal, then launch the `workforce-web` app from there.

## Local Authentication (Dex)
Dex is configured for local development in AppHost.

The Blazor web app uses these local auth routes:
- Sign in: `/authentication/login`
- Sign out: `/authentication/logout`

Default local test user from Dex config:
- Username: `admin`
- Password: `password`

Relevant config file:
- `src/aspire/Workforce.AppHost/dex-config.yaml`

## Build, Test, and Stop
Build:

```bash
dotnet build
```

Run tests:

```bash
dotnet test
```

Stop Aspire stack (if using Aspire CLI):

```bash
aspire stop --apphost src/aspire/Workforce.AppHost/Workforce.AppHost.csproj
```

## Functional Notes
- Employees can be added and filtered by skill.
- Skills are managed in the Skills page (reload/create).
- Skill assignment is performed from the Skills UI flow.
- Sorting is performed by the API rather than page-level sorting.
- The web app forwards backend calls under `/api/employees` and `/api/skills` to avoid route collisions with Blazor pages.

## Documentation Notes
- API endpoints are defined under `src/backend/Workforce.API/Endpoints`.
- Domain rules and value objects are under `src/backend/Workforce.Domain/Model`.
- UI pages are under `src/frontend/Workforce.Web/Workforce.Web.Client/Pages`.

## Prompt / Agent Usage Disclosure
This solution was developed with AI-assisted tooling (GitHub Copilot chat/agent workflow) for:
- refactoring,
- endpoint and UI wiring changes,
- iterative debugging and build verification,
- documentation drafting.

The developer reviewed and accepted all code and documentation changes before finalizing.

The following work was not done by the agent:
- Domain modeling decisions (entities, value objects, invariants, and boundaries).
- API interface design decisions (resource shape, endpoint semantics, and contracts).
- Solution structure and overall architecture decisions.
- High-level technical design/trade-off decisions.