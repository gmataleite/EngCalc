export type UnitSystem = "metric" | "imperial"
export type ShellType = "cylindrical" | "spherical"
export type CalculationMode = "min_thickness" | "mawp"

export interface MaterialOption {
  id: string
  label: string
  specification?: string
  grade?: string | null
  productForm?: string | null
  maxTemperatureLimit?: number
}

export interface SelectOption {
  id: string
  label: string
}

export interface ShellThicknessOptionsResponse {
  jointTypes: SelectOption[]
  rtOptions: SelectOption[]
}

export interface ShellThicknessCalculationRequest {
  unitSystem: UnitSystem
  shellType: ShellType
  mode: CalculationMode
  materialId: string
  designTemperature: number
  insideRadius: number
  internalPressure?: number
  thickness?: number
  jointTypeId: string
  rtId: string
  corrosionAllowance: number
  allowableStressOverride?: number
}

export interface ShellThicknessCalculationResponse {
  unitSystem: UnitSystem
  lengthUnit: string
  pressureUnit: string
  temperatureUnit: string
  jointEfficiency: number
  jointEfficiencyPercent: number
  allowableStress: number
  isManualAllowableStress: boolean
  circumferentialThickness: number
  longitudinalThickness: number
  governingRequiredThickness: number
  corrosionAllowance: number
  totalMinimumThickness: number
  circumferentialFormulaType: string
  longitudinalFormulaType: string
  breakdown: string
  warnings: string[]
}
