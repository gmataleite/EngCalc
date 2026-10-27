import type { SelectOption, UnitSystem } from "@/types/common"
import type { CalculationMode } from "@/features/pressure-vessels/shell-thickness/types"

export type HeadType =
  "ellipsoidal" | "torispherical" | "hemispherical" | "conical"

export interface HeadThicknessOptionsResponse {
  headTypes: SelectOption[]
  jointTypes: SelectOption[]
  rtOptions: SelectOption[]
}

export interface HeadThicknessCalculationRequest {
  unitSystem: UnitSystem
  headType: HeadType
  mode: CalculationMode
  materialId: string
  designTemperature: number
  internalPressure?: number
  thickness?: number
  insideDiameter?: number
  headHeight?: number
  crownRadius?: number
  knuckleRadius?: number
  insideRadius?: number
  jointTypeId: string
  rtId: string
  corrosionAllowance: number
  allowableStressOverride?: number
}

export interface HeadThicknessCalculationResponse {
  unitSystem: UnitSystem
  headType: HeadType
  activeLogic: string
  lengthUnit: string
  pressureUnit: string
  temperatureUnit: string
  jointEfficiency: number
  jointEfficiencyPercent: number
  allowableStress: number
  isManualAllowableStress: boolean
  requiredThicknessWithoutCa: number
  corrosionAllowance: number
  totalMinimumThickness: number
  referenceLatex: string
  breakdown: string
  warnings: string[]
  mode?: CalculationMode
  corrodedThickness?: number
  maximumAllowableWorkingPressure?: number
}
