import type { SelectOption } from "@/types/common"

export type TankHeadTypes =
  "flat" | "conical" | "hemispherical" | "ellipsoidal" | "torispherical"

export type TankOrientationTypes = "horizontal" | "vertical"

export interface TankOptionsResponse {
  tankHeadTypes: SelectOption[]
  tankOrientationTypes: SelectOption[]
}

export interface TankVolumeRequest {
  internalDiameter: number
  internalBodyHeight: number
  bottomType: TankHeadTypes
  bottomHeight: number | null
  bottomAngle: number | null
  headType: TankHeadTypes
  headHeight: number | null
  headAngle: number | null
  productHeight: number | null
  tankOrientation: TankOrientationTypes
}

export interface TankVolumeResponse {
  internalVolume: number
  productVolume: number
}
