# EngCalc Roadmap

This document tracks the planned epics and milestones for the **EngCalc** calculation engine and web application.

## Module 1: Pressure Vessels — ASME VIII Div. 1 (`PressureVessels`)
- [x] Cylindrical Shells — Minimum Required Thickness (UG-27 / Mandatory Appendix 1-2)
- [x] Cylindrical Shells — Solve for Maximum Allowable Working Pressure (MAWP) (UG-27 / Appendix 1-2)
- [ ] **To-Do:** Spherical Shells — Minimum Required Thickness & MAWP (UG-27(d) / Appendix 1-3)
- [x] Ellipsoidal Heads — Minimum Required Thickness (UG-32(c) / Appendix 1-4)
- [x] Torispherical Heads — Minimum Required Thickness (UG-32(d) / Appendix 1-4)
- [x] Hemispherical Heads — Minimum Required Thickness (UG-32(e) / Appendix 1-3)
- [ ] **To-Do:** Conical Heads & Sections — Minimum Required Thickness & MAWP (UG-32(f) / Appendix 1-4)
- [x] Formed Heads — Solve for Maximum Allowable Working Pressure (MAWP) (UG-32)
- [ ] **External Pressure & Vacuum (UG-28 / UG-33):** Iterative algorithm for shell and head buckling verification
- [ ] **Stiffening Rings (UG-29):** Required moment of inertia sizing for external pressure stiffening rings

## Module 2: Shafts & Machine Elements (`Shafts`)
- [ ] Solid & Hollow Shaft Sizing under combined torsion and bending (ASME B106.1M / Shigley)
- [ ] Fatigue endurance limit & safety factor calculation (Goodman, Gerber, Soderberg, von Mises)
- [ ] Keyway & Spline shear/bearing stress verification

## Module 3: Structural Engineering (`Structures`)
- [ ] Geometric Section Properties calculator (I-beam, channel, hollow rectangular/circular sections)
- [ ] Beam deflection, shear force, and bending moment analysis
- [ ] Column buckling verification (Euler / Johnson)

## Module 4: Engineering Unit Conversions (`UnitConversions`)
- [ ] Dedicated interactive unit conversion utility powered by `UnitsNet` (Pressure, Stress, Length, Force, Torque, Temperature, Density, Flow)

## Shared Platform & Infrastructure
- [x] Relational database schema and CSV seeding for ASME Section II Part D (Table 1A)
- [x] Linear temperature interpolation for allowable stress ($S$)
- [x] Modularization of solution from `VesselCalc` to `EngCalc`
- [ ] Engineering Calculation Report generation in PDF (QuestPDF)
- [ ] Multi-tenant custom material library
