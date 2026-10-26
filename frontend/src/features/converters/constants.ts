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
    temperature: {
      title: "Formula Reference (Temperature Conversions)",
      items: [
        {
          title: "Celsius to Fahrenheit",
          from: "°C",
          to: "°F",
          latex: String.raw`T_{(^\circ\text{F})} = T_{(^\circ\text{C})} \times \frac{9}{5} + 32`,
        },
        {
          title: "Fahrenheit to Celsius",
          from: "°F",
          to: "°C",
          latex: String.raw`T_{(^\circ\text{C})} = \left(T_{(^\circ\text{F})} - 32\right) \times \frac{5}{9}`,
        },
        {
          title: "Celsius to Kelvin",
          from: "°C",
          to: "K",
          latex: String.raw`T_{(\text{K})} = T_{(^\circ\text{C})} + 273.15`,
        },
        {
          title: "Kelvin to Celsius",
          from: "K",
          to: "°C",
          latex: String.raw`T_{(^\circ\text{C})} = T_{(\text{K})} - 273.15`,
        },
        {
          title: "Fahrenheit to Kelvin",
          from: "°F",
          to: "K",
          latex: String.raw`T_{(\text{K})} = \left(T_{(^\circ\text{F})} - 32\right) \times \frac{5}{9} + 273.15`,
        },
        {
          title: "Kelvin to Fahrenheit",
          from: "K",
          to: "°F",
          latex: String.raw`T_{(^\circ\text{F})} = \left(T_{(\text{K})} - 273.15\right) \times \frac{9}{5} + 32`,
        },
      ],
    },
    pressure: {
      title: "Formula Reference (Pressure Conversions)",
      items: [
        {
          title: "Pascal to Bar",
          from: "Pa",
          to: "bar",
          latex: String.raw`P_{(\text{bar})} = \frac{P_{(\text{Pa})}}{10^5}`,
        },
        {
          title: "Bar to Pascal",
          from: "bar",
          to: "Pa",
          latex: String.raw`P_{(\text{Pa})} = P_{(\text{bar})} \times 10^5`,
        },
        {
          title: "Pascal to PSI",
          from: "Pa",
          to: "psi",
          latex: String.raw`P_{(\text{psi})} = \frac{P_{(\text{Pa})}}{6894.757}`,
        },
        {
          title: "PSI to Pascal",
          from: "psi",
          to: "Pa",
          latex: String.raw`P_{(\text{Pa})} = P_{(\text{psi})} \times 6894.757`,
        },
        {
          title: "Pascal to N/m²",
          from: "Pa",
          to: "N/m²",
          latex: String.raw`P_{(\text{N/m}^2)} = P_{(\text{Pa})}`,
        },
        {
          title: "N/m² to Pascal",
          from: "N/m²",
          to: "Pa",
          latex: String.raw`P_{(\text{Pa})} = P_{(\text{N/m}^2)}`,
        },
      ],
    },
    volume: {
      title: "Formula Reference (Volume Conversions)",
      items: [
        {
          title: "Cubic Meters to Liters",
          from: "m3",
          to: "L",
          latex: String.raw`V_{(\text{L})} = V_{(\text{m}^3)} \times 1000`,
        },
        {
          title: "Liters to Cubic Meters",
          from: "L",
          to: "m3",
          latex: String.raw`V_{(\text{m}^3)} = \frac{V_{(\text{L})}}{1000}`,
        },
        {
          title: "Cubic Meters to Gallons",
          from: "m3",
          to: "gal",
          latex: String.raw`V_{(\text{gal})} = V_{(\text{m}^3)} \times 264.172`,
        },
        {
          title: "Gallons to Cubic Meters",
          from: "gal",
          to: "m3",
          latex: String.raw`V_{(\text{m}^3)} = \frac{V_{(\text{gal})}}{264.172}`,
        },
        {
          title: "Liters to Gallons",
          from: "L",
          to: "gal",
          latex: String.raw`V_{(\text{gal})} = \frac{V_{(\text{L})}}{3.78541}`,
        },
        {
          title: "Gallons to Liters",
          from: "gal",
          to: "L",
          latex: String.raw`V_{(\text{L})} = V_{(\text{gal})} \times 3.78541`,
        },
      ],
    },
  },
} as const
