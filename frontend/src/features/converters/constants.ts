export const CONVERTER_TEXTS = {
  pageTitle: "Units Converter",
  pageSubtitle: "Convert between different units of measurement",

  topConfig: {
    physicalPropertyLabel: "Physical Property",
    physicalPropertyPlaceholder: "Select physical property...",
    physicalPropertyTypeOptions: {
      temperature: "Temperature",
      pressure: "Pressure",
      volume: "Volume",
    },
  },

  converter: {
    cardTitle: "Unit Conversion",
    cardDescription:
      "Select the units and enter the value to perform real-time conversion.",
    initialValueLabel: (unit?: string) =>
      unit ? `Initial Value (${unit})` : "Initial Value",
    initialValuePlaceholder: "Enter initial value...",
    initialUnitLabel: "From Unit",
    initialUnitPlaceholder: "Select initial unit...",
    targetUnitLabel: "To Unit",
    targetUnitPlaceholder: "Select target unit...",
    swapUnitsTooltip: "Swap units",
    convertedValueCardLabel: "Converted Value",
    calculatingMessage: "Converting...",
    sameUnitWarning: "Initial and target units must be different.",
  },

  units: {
    temperature: {
      celsius: "Celsius (°C)",
      fahrenheit: "Fahrenheit (°F)",
      kelvin: "Kelvin (K)",
    },
    pressure: {
      pascal: "Pascal (Pa)",
      bar: "Bar (bar)",
      psi: "Pounds per square inch (psi)",
      newtonPerSquareMeter: "Newton per square meter (N/m²)",
    },
    volume: {
      cubicMeters: "Cubic Meters (m³)",
      liters: "Liters (L)",
      gallons: "Gallons (gal)",
    },
  },

  formulaReference: {
    title: "Formula Reference (Temperature Conversions)",
    items: [
      {
        title: "Celsius to Fahrenheit",
        latex: String.raw`T_{(^\circ\text{F})} = T_{(^\circ\text{C})} \times \frac{9}{5} + 32`,
      },
      {
        title: "Fahrenheit to Celsius",
        latex: String.raw`T_{(^\circ\text{C})} = \left(T_{(^\circ\text{F})} - 32\right) \times \frac{5}{9}`,
      },
      {
        title: "Celsius to Kelvin",
        latex: String.raw`T_{(\text{K})} = T_{(^\circ\text{C})} + 273.15`,
      },
      {
        title: "Kelvin to Celsius",
        latex: String.raw`T_{(^\circ\text{C})} = T_{(\text{K})} - 273.15`,
      },
      {
        title: "Fahrenheit to Kelvin",
        latex: String.raw`T_{(\text{K})} = \left(T_{(^\circ\text{F})} - 32\right) \times \frac{5}{9} + 273.15`,
      },
      {
        title: "Kelvin to Fahrenheit",
        latex: String.raw`T_{(^\circ\text{F})} = \left(T_{(\text{K})} - 273.15\right) \times \frac{9}{5} + 32`,
      },
    ],
  },
} as const
