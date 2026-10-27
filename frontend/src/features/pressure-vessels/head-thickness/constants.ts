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
    modeLabel: "Calculation Mode",
    modeOptions: {
      minThickness: "Solve for Minimum Thickness (t)",
      mawp: "Solve for Internal Design Pressure (MAWP)",
    },
  },

  section2: {
    materialColumnTitle: "Material & Environment",
    materialLabel: "Material Specification (Searchable)",
    materialPlaceholder: "Select material specification...",
    materialSearchPlaceholder: "Search ASME material...",
    materialEmptyText: "No matching material found.",
    designTempLabel: (unit: string) => `Design Temperature, T (${unit})`,

    geometryColumnTitle: "Geometry & Target",
    insideDiameterLabel: (unit: string) =>
      `Inside Diameter of Head Skirt, D (${unit})`,
    crownRadiusLabel: (unit: string) => `Inside Crown Radius, L (${unit})`,
    knuckleRadiusLabel: (unit: string) => `Inside Knuckle Radius, r (${unit})`,
    insideRadiusLabel: (unit: string) => `Inside Spherical Radius, L (${unit})`,
    internalPressureLabel: (unit: string) =>
      `Internal Design Pressure, P (${unit})`,
    nominalThicknessLabel: (unit: string) =>
      `Nominal Head Thickness, t (${unit})`,

    fabricationColumnTitle: "Fabrication & Corrosion",
    jointTypeLabel: "Welding Joint Type",
    rtLabel: "Radiographic Examination (RT)",
    corrosionAllowanceLabel: (unit: string) =>
      `Corrosion Allowance, CA (${unit})`,
  },

  section3: {
    title: "Allowable Stress (S) Configuration",
    manualOverrideLabel: "Manual Override for Allowable Stress (S)",
    autoInterpolatedMessage:
      "Allowable Stress is automatically interpolated from the database.",
    manualInputLabel: (unit: string) => `Custom Allowable Stress, S (${unit})`,
  },

  section4: {
    title: "Calculation Results",
    jointEfficiencyCardLabel: "Joint Efficiency (E)",
    allowableStressCardLabel: "Allowable Stress (S)",
    minThicknessCardLabel: "Total Minimum Thickness (t)",
    mawpCardLabel: "Maximum Allowable Working Pressure (MAWP)",
    calculatingMessage: "Updating calculation...",
  },

  formulaReference: {
    title:
      "Formula Reference (ASME BPVC Section VIII Div 1 — UG-32 & Appendix 1)",
    groups: [
      {
        groupTitle: "Ellipsoidal Head (UG-32(c) & Appendix 1-4(c))",
        items: [
          {
            title: "Thickness (t) - Ellipsoidal",
            latex: String.raw`t = \frac{P \cdot D}{2 \cdot S \cdot E - 0.2 \cdot P}\;\text{(2:1)} \;\Big|\; t = \frac{P \cdot D \cdot K}{2 \cdot S \cdot E - 0.2 \cdot P}\;\text{(Custom)}`,
          },
          {
            title: "Pressure (P) - Ellipsoidal",
            latex: String.raw`P = \frac{2 \cdot S \cdot E \cdot t}{D + 0.2 \cdot t}\;\text{(2:1)} \;\Big|\; P = \frac{2 \cdot S \cdot E \cdot t}{D \cdot K + 0.2 \cdot t},\; K = \frac{1}{6}\left[2 + \left(\frac{D}{2h}\right)^2\right]`,
          },
        ],
      },
      {
        groupTitle: "Torispherical Head (UG-32(d) & Appendix 1-4(d))",
        items: [
          {
            title: "Thickness (t) - Torispherical",
            latex: String.raw`t = \frac{P \cdot L \cdot M}{2 \cdot S \cdot E - 0.2 \cdot P},\; M = \frac{1}{4}\left(3 + \sqrt{\frac{L}{r}}\right)`,
          },
          {
            title: "Pressure (P) - Torispherical",
            latex: String.raw`P = \frac{2 \cdot S \cdot E \cdot t}{L \cdot M + 0.2 \cdot t},\; M = \frac{1}{4}\left(3 + \sqrt{\frac{L}{r}}\right)`,
          },
        ],
      },
      {
        groupTitle: "Hemispherical Head (UG-32(e) & Appendix 1-3)",
        items: [
          {
            title: "Thickness (t) - Hemispherical",
            latex: String.raw`t = \frac{P \cdot L}{2 \cdot S \cdot E - 0.2 \cdot P}\;\text{(Thin)} \;\Big|\; t = L \left( \sqrt[3]{\frac{2(S \cdot E + P)}{2 \cdot S \cdot E - P}} - 1 \right)\;\text{(Thick)}`,
          },
          {
            title: "Pressure (P) - Hemispherical",
            latex: String.raw`P = \frac{2 \cdot S \cdot E \cdot t}{L + 0.2 \cdot t}\;\text{(Thin)} \;\Big|\; P = 2 \cdot S \cdot E \left(\frac{Y - 1}{Y + 2}\right),\; Y = \left(\frac{L + t}{L}\right)^3\;\text{(Thick)}`,
          },
        ],
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
