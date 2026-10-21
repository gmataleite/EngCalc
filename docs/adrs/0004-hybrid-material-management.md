# ADR 0004: Hybrid Material Database Management

**Status:** Accepted  
**Date:** 2026-04-09

## Context
Vessel thickness calculations rely on material allowable stress ($S$) as a function of design temperature (ASME Section II Part D, Table 1A). While standard ASME materials should be pre-populated via database seeding, engineers also need the ability to override allowable stress or define custom materials for specialized simulations.

## Decision
Adopt a PostgreSQL relational model backed by Entity Framework Core (`Material` and `MaterialTemperatureProperty`) seeded from static CSV files (`materials.csv`), combined with linear temperature interpolation (`Material.GetAllowableStress`) and support for manual allowable stress overrides in the calculation API.

## Consequences
* **Positive:**
  * Fast, accurate automatic interpolation from ASME Section II Part D data.
  * Full flexibility for engineers to specify a manual allowable stress override when simulating non-standard alloys, accompanied by an explicit audit warning in the calculation output.
* **Negative:**
  * Future multi-tenant custom material storage will require EF Core query filters to isolate tenant-specific materials.
