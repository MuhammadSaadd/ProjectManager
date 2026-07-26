# Project Manager

Simple Task Manager monorepo: ASP.NET Core API (DDD + CQRS), React frontend, SQL Server.

## Monorepo structure

```
ProjectManager/
├── backend/        # C# solution (Domain, Application, Infrastructure, Api)
├── frontend/web/   # React + Vite + Tailwind + Flowbite
├── schemas/        # Generated OpenAPI (api.json)
└── docker-compose.yml
```

## Prerequisites

- [proto](https://moonrepo.dev/proto) (or moon + node/pnpm already on PATH)
- .NET SDK 10+
- Docker (SQL Server)

```bash
proto install
```

## Quick start

```bash
# Infrastructure
docker compose up -d

# Backend (after solution is implemented)
moon backend:restore
moon backend:migrate
moon api:dev          # http://localhost:3100

# Frontend (after app is scaffolded)
moon web:generate-dtos
moon web:dev          # http://localhost:3000
```

## Ports

| Port | Service        |
|------|----------------|
| 3000 | Frontend (web) |
| 3100 | API            |
| 1433 | SQL Server     |
| 3200 | Adminer        |

## Status

Scaffolding in progress — see commits for step-by-step progress.

## Assumptions / design decisions

- Moon-managed monorepo patterned after BytesMaestros/menuzi layout
- C# DDD + CQRS (MediatR), EF Core Code First — no event sourcing
- OpenAPI → `schemas/api.json` → Orval → Zod client for the frontend
- Tailwind utilities + Flowbite React only (no custom CSS files)

## Out of scope

Auth, microservice split, heavy polish, production deploy.
