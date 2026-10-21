# ADR 0002: Strategy Pattern for Shell Thickness Calculations

**Status:** Accepted  
**Date:** 2026-03-18

## Context
Paragraph UG-27 of ASME VIII Div. 1 defines cylindrical shell equations for circumferential and longitudinal stresses. However, if internal pressure exceeds specific limits (e.g., $P > 0.385 \cdot S \cdot E$) or if required wall thickness exceeds half the inside radius ($t > 0.5 \cdot R$), the component is classified as a "thick shell," requiring the Lamé equations from *Mandatory Appendix 1-2*. Embedding all regime checks and algebraic formulas inside a single monolithic method in `CylindricalShell` produces fragile, hard-to-maintain code.

## Decision
Implement the **Strategy** design pattern in the calculation domain.
A dedicated interface `IShellThicknessCalculator` was introduced with specific implementations (`CircumferentialThinShellCalculator`, `CircumferentialThickShellCalculator`, `LongitudinalThinShellCalculator`, `LongitudinalThickShellCalculator`). The domain entity orchestrates execution by evaluating threshold conditions and dynamically selecting the appropriate calculator strategy.

## Consequences
* **Positive:**
  * Adheres to the Open/Closed Principle (OCP) and Single Responsibility Principle (SRP).
  * Each algebraic formula is isolated and independently testable.
* **Negative:**
  * Increases the number of small classes in the Domain layer.
