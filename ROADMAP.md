# Vessel-Calc Roadmap

This document tracks the planned epics and milestones for the calculation engine and web application.

## Phase 1: Core ASME VIII Div. 1 Domain (In Progress)
- [x] Cylindrical Shells — Minimum Required Thickness (UG-27 / Mandatory Appendix 1-2)
- [ ] **To-Do:** Cylindrical Shells — Solve for Maximum Allowable Working Pressure (MAWP) (UG-27 / Appendix 1-2)
- [ ] **To-Do:** Spherical Shells — Minimum Required Thickness & MAWP (UG-27(d) / Appendix 1-3)
- [x] Ellipsoidal Heads — Domain Model (UG-32 / Appendix 1-4)
- [x] Torispherical Heads — Domain Model (UG-32 / Appendix 1-4)
- [x] Hemispherical Heads — Domain Model (UG-32 / Appendix 1-3)

## Phase 2: Material Management & Data Persistence
- [x] Relational database schema and CSV seeding for ASME Section II Part D (Table 1A)
- [x] Linear temperature interpolation for allowable stress ($S$)
- [ ] EF Core query filters for multi-tenant isolation of user-defined custom materials

## Phase 3: Presentation & Reporting
- [x] Migration of frontend architecture from Blazor WebAssembly to React + Vite + TypeScript (ADR 0006)
- [x] Page 01: Shell Thickness Under Internal Pressure (`UG-27`) in React
- [ ] **To-Do:** Expose Spherical Shell and MAWP modes in the Shell Thickness (`UG-27`) page and API
- [ ] Formed Heads (`UG-32`) calculation pages in React
- [ ] Engineering Calculation Report generation in PDF (QuestPDF)

## Phase 4: Structural Stability & Buckling
- [ ] **External Pressure & Vacuum Module (UG-28 / UG-33):** Iterative algorithm for shell and head buckling verification
- [ ] **ASME Section II-D Chart Digitization:** Bi-logarithmic interpolation service for Geometric Factor ($A$) and Material Factor ($B$) extraction
- [ ] **Stiffening Rings (UG-29):** Required moment of inertia sizing for external pressure stiffening rings
