import type { SelectOption } from "@/types/common"

export type PhysicalPropertyType = "temperature" //| "pressure" | "volume"

export type TemperatureUnit = "°C" | "°F" | "K"

// export type PressureUnit = "Pa" | "kPa" | "MPa" | "bar" | "psi" | "atm"

// export type VolumeUnit = "m3" | "L" | "cm3" | "ft3" | "in3" | "gal"

export interface ConverterOptionsResponse {
  PhysicalPropertiesTypes: SelectOption[]
  TemperatureUnits: SelectOption[]
  // PressureUnits: SelectOption[]
  // VolumeUnits: SelectOption[]
}

export interface ConverterRequest {
  physicalPropertyType: PhysicalPropertyType
  initialValue: number
  initialUnit: TemperatureUnit //| PressureUnit | VolumeUnit
  targetUnit: TemperatureUnit //| PressureUnit | VolumeUnit
}

export interface ConverterResponse {
  convertedValue: number
  convertedUnit: TemperatureUnit //| PressureUnit | VolumeUnit
}
