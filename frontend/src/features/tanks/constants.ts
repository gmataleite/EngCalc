export type TankDimensionMode = "height" | "angle"

export const DEFAULT_TANK_OPTIONS = {
  headTypes: [
    { id: "Flat", label: "Flat head" },
    { id: "Conical", label: "Conical head" },
    { id: "Hemispherical", label: "Hemispherical head" },
    { id: "Ellipsoidal", label: "Ellipsoidal head" },
    { id: "Torispherical", label: "Torispherical head" },
  ],
  orientations: [
    { id: "Horizontal", label: "Horizontal" },
    { id: "Vertical", label: "Vertical" },
  ],
} as const

export const TANK_VOLUME_TEXTS = {
  pageTitle: "Tank volume calculator",
  pageSubtitle:
    "Calculate the internal and product volume of a cylindrical tank.",
  configuration: {
    title: "Tank configuration",
    description:
      "Enter the tank dimensions and select the geometry of each head.",
  },
  fields: {
    internalDiameter: "Internal diameter (mm)",
    internalBodyHeight: "Internal body height (mm)",
    productHeight: "Product height (mm)",
    orientation: "Tank orientation",
  },
  bottom: {
    title: "Bottom",
    typeLabel: "Bottom type",
    dimensionLabel: "Conical bottom dimension",
    heightLabel: "Height (mm)",
    angleLabel: "Angle (°)",
  },
  head: {
    title: "Top head",
    typeLabel: "Head type",
    dimensionLabel: "Conical head dimension",
    heightLabel: "Height (mm)",
    angleLabel: "Angle (°)",
  },
  results: {
    title: "Calculation results",
    internalVolume: "Internal volume",
    productVolume: "Product volume",
    calculating: "Updating calculation...",
  },
  formulaReference: {
    title: "Formula reference",
    items: [
      {
        title: "Cylindrical body volume",
        latex: String.raw`V_{\text{body}} = \pi \left(\frac{D}{2}\right)^2 H`,
      },
      {
        title: "Conical head volume",
        latex: String.raw`V_{\text{cone}} = \frac{1}{3}\pi \left(\frac{D}{2}\right)^2 h`,
      },
    ],
  },
} as const
