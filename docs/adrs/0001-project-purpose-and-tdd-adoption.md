# ADR 0001: Project Purpose and Adoption of TDD

**Status:** Accepted  
**Date:** 2026-03-06

## Context
The Vessel-Calc project was created to combine normative mechanical engineering knowledge (ASME VIII Div. 1) with modern enterprise software engineering practices. Engineering software handles safety-critical calculations where defects pose risks to human life and equipment integrity. To remain reliable for independent engineers and industrial teams, physical equations must be protected against regressions as the codebase evolves.

## Decision
1. **Purpose:** The system is developed as a multi-tenant SaaS focused on rigorous mechanical calculation automation and engineering report generation.
2. **Quality Engineering:** The calculation core (Domain Layer) is developed under strict **Test-Driven Development (TDD)**. Unit tests (via xUnit) drive the design of physical entities (Shells, Heads, Nozzles), ensuring every ASME code rule has test coverage before functional implementation.

## Consequences
* **Positive:**
  * Regression-proof calculation engine. Future additions remain verified against existing test suites.
  * TDD encourages cohesive, decoupled classes aligned with SOLID principles.
  * Living documentation of expected normative behavior through parameterized unit tests.
* **Negative:**
  * Higher initial design and development time.
  * Requires verified reference calculation cases extracted from validated engineering design reports.
