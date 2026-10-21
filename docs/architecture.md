# System Architecture

**Vessel-Calc** uses **Clean Architecture** to manage the complexity inherent in ASME engineering calculations. The goal of this topology is to ensure that the Calculation Engine (Domain) remains independent of web frameworks, user interfaces, and databases.

## Solution Structure

The repository is organized into the following layers:

1. **VesselCalc.Domain (Core):**
   * Contains engineering entities (`CylindricalShell`, `EllipsoidalHead`, `HemisphericalHead`, `TorisphericalHead`, `Material`), Value Objects, and calculation interfaces (`IShellThicknessCalculator`).
   * **Zero external framework dependencies** (only `UnitsNet` for strict dimensional safety).
   * Uses the **Strategy** pattern to dynamically switch between thin-shell equations (UG-27) and thick-shell equations (Mandatory Appendix 1-2).

2. **VesselCalc.Application (Use Cases):**
   * Orchestrates user workflows. Contains DTOs, service interfaces (`IShellThicknessService`, `IMaterialProvider`), unit system conversions, ASME Table UW-12 joint efficiency resolution, and calculation services that connect material allowable stresses to Domain equations.

3. **VesselCalc.Infrastructure (External Adapters):**
   * Implements data access via **Entity Framework Core** with the **PostgreSQL (Npgsql)** provider.
   * Manages initial normative data seeding (`DatabaseSeeder`) from static CSV files (ASME Section II Part D) and provides a resilient CSV fallback when running without a database container.

4. **VesselCalc.API (Backend Presentation):**
   * ASP.NET Core Web API REST endpoints (`/api/materials`, `/api/calculations/shell-thickness`) with CORS configured for the React SPA.

5. **frontend (Frontend Presentation):**
   * Single Page Application (SPA) built with **React + Vite + TypeScript + Tailwind CSS v4 + shadcn/ui**, consuming the REST API reactively via TanStack Query.
   * Contains zero engineering calculation formulas; all normative math executes exclusively in `VesselCalc.Domain`.
