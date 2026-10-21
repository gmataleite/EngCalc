export const HEAD_THICKNESS_TEXTS = {
  pageTitle: "ASME BPVC Section VIII Div 1 UG-32",
  pageSubtitle: "Formed Heads, And Sections, Pressure on Concave Side",

  topConfig: {
    unitSystemLabel: "Unit System",
    unitOptions: {
      imperial: "Imperial (psi, inch, °F)",
      metric: "Metric (MPa, mm, °C)",
    },
    headTypeLabel: "Formed Head Type",
    headTypeOptions: {
      ellipsoidal: "Ellipsoidal",
      torispherical: "Torispherical",
      hemispherical: "Hemispherical",
      conical: "Conical (Planned)",
    },
  },

  step1: {
    title: "Step 1: Material & Environmental Conditions",
    materialLabel: "Material Specification (Searchable)",
    materialPlaceholder: "Select material specification...",
    materialSearchPlaceholder: "Search ASME material...",
    materialEmptyText: "No matching material found.",
    designTempLabel: (unit: string) => `Design Temperature (${unit})`,

    allowableStressTitle: "Allowable Stress (S)",
    manualOverrideLabel: "Manual Override for Allowable Stress (S)",
    manualInputLabel: (unit: string) => `Custom Allowable Stress S (${unit})`,
    allowableStressCardLabel: "Allowable Stress (S) Used",
  },

  step2: {
    title: "Step 2: Welding Joint & Efficiency (E)",
    jointTypeLabel: "Welding Joint Description",
    rtLabel: "Radiographic Examination (RT)",
    jointEfficiencyCardLabel: "Joint Efficiency (E)",
  },

  step3: {
    title: "Step 3: Calculation Mode & Geometry",
    modeLabel: "Select Calculation Goal",
    modeOptions: {
      minThickness: "Calculate Minimum Thickness (t)",
      mawp: "Calculate Maximum Allowable Working Pressure (P) (Planned)",
    },
    corrosionAllowanceLabel: (unit: string) =>
      `Corrosion Allowance, CA (${unit})`,
  },

  step4: {
    activeLogicPrefix: "Active Logic:",
    insideDiameterLabel: (unit: string) =>
      `Inside Diameter of Head Skirt, D (${unit})`,
    crownRadiusLabel: (unit: string) => `Inside Crown Radius, L (${unit})`,
    knuckleRadiusLabel: (unit: string) => `Inside Knuckle Radius, r (${unit})`,
    insideRadiusLabel: (unit: string) => `Inside Spherical Radius, L (${unit})`,
    internalPressureLabel: (unit: string) =>
      `Internal Design Pressure, P (${unit})`,
    minThicknessCardLabel: "Minimum Thickness (t)",
    referencePrefix: "Reference:",
    calculatingMessage: "Updating calculation...",
  },

  formulaReference: {
    title:
      "Formula Reference (ASME BPVC Section VIII Div 1 — UG-32 & Appendix 1)",
    items: [
      {
        title: "Ellipsoidal Head (2:1 Standard — UG-32(c))",
        latex: String.raw`t = \frac{P \cdot D}{2SE - 0.2P} + CA`,
      },
      {
        title: "Ellipsoidal Head (Custom Ratio — Appendix 1-4(c))",
        latex: String.raw`t = \frac{P \cdot D \cdot K}{2SE - 0.2P} + CA, \quad K = \frac{1}{6}\left[2 + \left(\frac{D}{2h}\right)^2\right]`,
      },
      {
        title: "Torispherical Head (Appendix 1-4(d))",
        latex: String.raw`t = \frac{P \cdot L \cdot M}{2SE - 0.2P} + CA, \quad M = \frac{1}{4}\left(3 + \sqrt{\frac{L}{r}}\right)`,
      },
      {
        title: "Hemispherical Head (UG-32(e) / Appendix 1-3)",
        latex: String.raw`t = \frac{P \cdot L}{2SE - 0.2P} + CA\;\text{(Thin)} \quad \Big| \quad t = L\left[\left(\frac{2SE + P}{2SE - P}\right)^{1/3} - 1\right] + CA\;\text{(Thick)}`,
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
