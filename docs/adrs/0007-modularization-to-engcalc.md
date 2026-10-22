# ADR 0007: Rebranding and Domain Modularization to EngCalc

## Status
Accepted

## Context
Originally conceived as `Vessel-Calc` focusing exclusively on ASME BPVC Section VIII Division 1 pressure vessel calculations, the product scope expanded to become a general engineering calculation platform covering multiple disciplines:
- **Pressure Vessels** (`PressureVessels` — ASME VIII Div. 1 shells, heads, external pressure)
- **Shafts & Machine Elements** (`Shafts` — torsion, bending, fatigue, keyways)
- **Structures** (`Structures` — beam deflection, section properties, buckling)
- **Unit Conversions** (`UnitConversions` — dimensional conversions across SI and Imperial)

Keeping the `VesselCalc` name and flat folder structure inside `Domain`, `Application`, `API`, and `frontend/src/features` would create naming confusion as non-vessel calculators are added.

## Decision
1. **Rename the solution and assemblies** from `VesselCalc.*` to `EngCalc.*` (`EngCalc.Domain`, `EngCalc.Application`, `EngCalc.Infrastructure`, `EngCalc.API`, `EngCalc.Tests`).
2. **Modularize by Engineering Domain** within each Clean Architecture layer:
   - Shared cross-cutting domain concepts (`Common/Guard`, `Constants/PhysicalConstants`, `Entities/Material`) remain at the root of `EngCalc.Domain` and `EngCalc.Application/Materials`.
   - Discipline-specific calculations are grouped in dedicated module folders and namespaces (e.g., `EngCalc.Domain.PressureVessels`, `EngCalc.Application.PressureVessels`, `EngCalc.API.Controllers.PressureVessels`, `frontend/src/features/pressure-vessels`).
3. **Decouple Material Queries:** Extracted `IMaterialService` / `MaterialService` in `EngCalc.Application.Materials` so `MaterialsController` no longer depends on pressure-vessel-specific services (`IShellThicknessService`).

## Consequences
* **Positive:** New engineering domains (`Shafts`, `Structures`, `UnitConversions`) can be added as self-contained vertical slices across `Domain`, `Application`, `API`, and `frontend/src/features` without polluting existing modules.
* **Positive:** Shared engineering infrastructure (such as the material database and `UnitsNet` dimensional safety) is reusable across all engineering modules.

