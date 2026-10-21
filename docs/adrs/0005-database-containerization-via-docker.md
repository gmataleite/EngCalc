# ADR 0005: PostgreSQL Database Containerization via Docker

**Status:** Accepted  
**Date:** 2026-04-16 (Updated 2026-10-19)

## Context
VesselCalc is an engineering SaaS that requires a relational database capable of high decimal precision (critical for ASME normative calculations) and strict referential integrity across Linux/Windows local development environments and CI/CD pipelines.

## Decision
Use **PostgreSQL 17** (`postgres:17-alpine`) via **Docker Compose** (`infra/docker-compose.dev.yml`) with the `Npgsql.EntityFrameworkCore.PostgreSQL` EF Core provider. Additionally, `DatabaseSeeder` and `MaterialProvider` include an automatic fallback to `Data/Seed/materials.csv` if the local PostgreSQL container is not running.

## Consequences
* **Positive:**
  * Lightweight container footprint (`postgres:17-alpine`) compared to SQL Server images.
  * First-class cross-platform support on Windows, Linux, and GitHub Actions CI.
  * Zero-friction local startup: developers can run the full stack with Docker Compose or immediately test the API using the seeded CSV fallback.
* **Negative:**
  * Requires Docker Desktop (or a local PostgreSQL instance) when testing EF Core migrations directly.

