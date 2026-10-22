# System Architecture

**EngCalc** uses **Clean Architecture** combined with **Domain Modularization** to manage the complexity inherent in multi-disciplinary engineering calculations. The goal of this topology is to ensure that the Calculation Engine (`EngCalc.Domain`) remains independent of web frameworks, user interfaces, and databases, while allowing new engineering disciplines (`PressureVessels`, `Shafts`, `Structures`, `UnitConversions`) to be added cleanly.

## Solution Structure

The repository is organized into the following layers:

1. **EngCalc.Domain (Core):**
   * Contains shared core concepts (`Common/Guard`, `Constants/PhysicalConstants`, `Entities/Material`) and domain-specific calculation modules:
     * `PressureVessels/`: Engineering entities (`CylindricalShell`, `EllipsoidalHead`, `HemisphericalHead`, `TorisphericalHead`), Value Objects, and calculation interfaces (`IShellThicknessCalculator`).
     * Planned modules: `Shafts/`, `Structures/`, `UnitConversions/`.
   * **Zero external framework dependencies** (only `UnitsNet` for strict dimensional safety).
   * Uses the **Strategy** pattern to dynamically switch between calculation regimes (e.g., thin-shell UG-27 vs. thick-shell Mandatory Appendix 1-2).

2. **EngCalc.Application (Use Cases):**
   * Orchestrates user workflows, organized by module (`Common/`, `Materials/`, `PressureVessels/`).
   * Contains DTOs, service interfaces (`IMaterialService`, `IMaterialProvider`, `IShellThicknessService`, `IHeadThicknessService`), unit system conversions, and calculation services that connect material properties to Domain equations.

3. **EngCalc.Infrastructure (External Adapters):**
   * Implements data access via **Entity Framework Core** with the **PostgreSQL (Npgsql)** provider.
   * Manages initial normative data seeding (`DatabaseSeeder`) from static CSV files (ASME Section II Part D) and provides a resilient CSV fallback when running without a database container.

4. **EngCalc.API (Backend Presentation):**
   * ASP.NET Core Web API REST endpoints organized by domain (`Controllers/MaterialsController`, `Controllers/PressureVessels/...`) with CORS configured for the React SPA.

5. **frontend (Frontend Presentation):**
   * Single Page Application (SPA) built with **React + Vite + TypeScript + Tailwind CSS v4 + shadcn/ui**, consuming the REST API reactively via TanStack Query.
   * Feature folders (`src/features/pressure-vessels/...`, `src/pages/pressure-vessels/...`) mirror the backend domain modules.
   * Contains zero engineering calculation formulas; all normative math executes exclusively in `EngCalc.Domain`.
