export type UnitSystem = "metric" | "imperial"

export interface SelectOption {
  id: string
  label: string
}

export interface MaterialOption {
  id: string
  label: string
  specification?: string
  grade?: string | null
  productForm?: string | null
  maxTemperatureLimit?: number
}

