# ADR 0003: Logical Separation Between Required and Nominal Thickness

**Status:** Accepted  
**Date:** 2026-04-09

## Context
Specifying a steel plate involves three distinct logical stages:
1. **Design Required Thickness ($t$):** Pure structural mechanics based on pressure, geometry, allowable stress, and joint efficiency (ASME equations).
2. **Total Minimum Required Thickness ($t + CA$):** Required thickness plus corrosion allowance ($CA$) and forming allowances (UG-79 / UG-16).
3. **Nominal (Commercial) Thickness:** Rounding up the minimum required thickness to the next commercially available plate thickness in the target market.

## Decision
Core Domain entities (`CylindricalShell`, `HemisphericalHead`, `EllipsoidalHead`, `TorisphericalHead`) are restricted strictly to **stages 1 and 2**, returning the exact analytical value via `CalculateMinimumRequiredThickness()`.
Stage 3 (commercial plate selection) is a supply-chain concern rather than physical mechanics and is delegated to an Application-layer service when needed.

## Consequences
* **Positive:**
  * Keeps the Core Domain mathematically pure and free of market-specific catalog logic.
  * Supports both Metric (mm) and Imperial (inch) workflows seamlessly.
* **Negative:**
  * Requires Application-layer orchestration to assemble final DTOs for the API and Frontend.
