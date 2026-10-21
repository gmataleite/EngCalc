# Modeling ASME Rules in the Domain

This document bridges mechanical engineering (ASME BPVC Section VIII Div. 1) and the object-oriented domain model.

## The Normative Challenge
The ASME code is not a simple linear formula sheet. It defines regimes, stress thresholds, and geometric limits. Translating this into software requires strict encapsulation so invalid inputs or out-of-bounds ratios cannot produce unsafe pressure vessel designs.

## Concept Mapping

### 1. Physical Quantities & Dimensional Safety
Unit conversions (`MPa`, `psi`, `mm`, `inch`, `°C`, `°F`) are a common source of critical bugs in engineering software.
* **Decision:** Use the `UnitsNet` library. Domain constructors and methods accept strongly typed physical quantities (`Pressure`, `Length`, `Density`) rather than raw `double` primitives.

### 2. The Thick-Shell Transition (UG-27 vs. Mandatory Appendix 1-2)
Primary thin-shell design equations (UG-27) lose accuracy when internal pressure exceeds $0.385 \cdot S \cdot E$ (circumferential) or $1.25 \cdot S \cdot E$ (longitudinal), or when the required thickness exceeds half the inside radius ($t > 0.5 \cdot R$).
* **Code Modeling:** The `CylindricalShell` entity evaluates thin-vs-thick boundary conditions internally. Using the *Strategy* pattern, it selects `CircumferentialThickShellCalculator` or `LongitudinalThickShellCalculator` at runtime, ensuring an automatic transition to Mandatory Appendix 1-2 (Lamé equations).

### 3. Corrosion Allowance ($CA$)
Corrosion allowance ($CA$) does not alter structural stress thresholds; it only increases the final required metal thickness.
* **Code Modeling:** $CA$ is validated in the entity constructor and added exclusively at the final stage of `CalculateMinimumRequiredThickness()`, preventing interference with ASME thin/thick regime boundary checks.
