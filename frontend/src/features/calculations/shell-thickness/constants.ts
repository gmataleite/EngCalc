export const SHELL_THICKNESS_TEXTS = {
  pageTitle:
    "ASME BPVC Section VIII Div 1 UG-27 Thickness of Shells Under Internal Pressure",
  pageSubtitle: "Design Calculations for Shells Under Internal Pressure",

  section1: {
    unitSystemLabel: "Unit System",
    unitOptions: {
      imperial: "Imperial (psi, inch, °F)",
      metric: "Metric (MPa, mm, °C)",
    },
    shellTypeLabel: "Shell Type",
    shellTypeOptions: {
      cylindrical: "Cylindrical Shell",
      spherical: "Spherical Shell (Planned)",
    },
    modeLabel: "Calculation Mode",
    modeOptions: {
      minThickness: "Solve for Minimum Thickness",
      mawp: "Solve for Internal Design Pressure (MAWP) (Planned)",
    },
  },

  section2: {
    materialColumnTitle: "Material & Environment",
    materialLabel: "Material Specification (Searchable)",
    materialPlaceholder: "Select material specification...",
    materialSearchPlaceholder: "Search ASME material...",
    materialEmptyText: "No matching material found.",
    designTempLabel: (unit: string) => `Design Temperature (${unit})`,

    geometryColumnTitle: "Geometry & Target",
    insideRadiusLabel: (unit: string) => `Inside Radius (${unit})`,
    internalPressureLabel: (unit: string) => `Internal Pressure (${unit})`,

    fabricationColumnTitle: "Fabrication & Corrosion",
    jointTypeLabel: "Welding Joint Type",
    rtLabel: "Radiographic Examination (RT)",
    corrosionAllowanceLabel: (unit: string) => `Corrosion Allowance (${unit})`,
  },

  section3: {
    title: "Allowable Stress (S) Configuration",
    manualOverrideLabel: "Manual Override for Allowable Stress (S)",
    autoInterpolatedMessage:
      "Allowable Stress is automatically interpolated from the database.",
    manualInputLabel: (unit: string) => `Custom Allowable Stress S (${unit})`,
    metricCardLabel: "Allowable Stress (S) Used",
  },

  section4: {
    title: "Calculation Results",
    jointEfficiencyCardLabel: "Joint Efficiency (E)",
    allowableStressCardLabel: "Allowable Stress (S)",
    totalThicknessCardLabel: "Total Minimum Thickness",
    calculatingMessage: "Updating calculation...",
  },

  section5: {
    title: "Formula Reference (ASME BPVC Section VIII Div 1)",
    items: [
      {
        title: "Thickness (t) - Circumferential",
        latex: String.raw`t = \frac{P \cdot R}{S \cdot E - 0.6 \cdot P}\;\text{(Thin)} \quad \Big| \quad t = R \left( \sqrt{\frac{S \cdot E + P}{S \cdot E - P}} - 1 \right)\;\text{(Thick)}`,
      },
      {
        title: "Thickness (t) - Longitudinal",
        latex: String.raw`t = \frac{P \cdot R}{2 \cdot S \cdot E + 0.4 \cdot P}\;\text{(Thin)} \quad \Big| \quad t = R \left( \sqrt{\frac{P}{S \cdot E} + 1} - 1 \right)\;\text{(Thick)}`,
      },
      {
        title: "Pressure (P) - Circumferential",
        latex: String.raw`P = \frac{S \cdot E \cdot t}{R + 0.6 \cdot t}\;\text{(Thin)} \quad \Big| \quad P = S \cdot E \frac{Z - 1}{Z + 1},\; Z = \left( \frac{R + t}{R} \right)^2\;\text{(Thick)}`,
      },
      {
        title: "Pressure (P) - Longitudinal",
        latex: String.raw`P = \frac{2 \cdot S \cdot E \cdot t}{R - 0.4 \cdot t}\;\text{(Thin)} \quad \Big| \quad P = S \cdot E (Z - 1)\;\text{(Thick)}`,
      },
    ],
  },

  units: {
    metric: {
      length: "mm",
      pressure: "MPa",
      temperature: "°C",
    },
    imperial: {
      length: "inch",
      pressure: "psi",
      temperature: "°F",
    },
  },
} as const
