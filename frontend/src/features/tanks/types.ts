import type { SelectOption } from "@/types/common"

export type TankHeadTypes = "Flat" | "Conical" | "Hemispherical" | "Ellipsoidal" | "Torispherical"

export type TankOrientationTypes = "Horizontal" | "Vertical"

export interface TankOptionsResponse {
  TankHeadType: SelectOption[]
  TankOrientation: SelectOption[]
}

export interface TankVolumeRequest {
  internalDiameter: number
  internalBodyHeight: number
  bottomType: TankHeadTypes
  bottomHeight: number
  bottomAngle: number
  headType: TankHeadTypes
  headHeight: number
  headAngle: number
  productHeight: number
  tankOrientation: TankOrientationTypes
}

export interface TankVolumeResponse {
  internalVolume: number
  productVolume: number
}
