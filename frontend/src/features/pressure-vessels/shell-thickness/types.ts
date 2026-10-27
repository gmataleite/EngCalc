import type { MaterialOption, SelectOption, UnitSystem } from "@/types/common"

export type { MaterialOption, SelectOption, UnitSystem }
export type ShellType = "cylindrical" | "spherical"
export type CalculationMode = "min_thickness" | "mawp"

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
  mode?: CalculationMode
  circumferentialMawp?: number
  longitudinalMawp?: number
  governingMawp?: number
  corrodedThickness?: number
}
