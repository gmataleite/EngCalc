import { api } from "@/services/api"
import type {
  TankVolumeRequest,
  TankVolumeResponse,
  TankOptionsResponse,
} from "./types"

export async function fetchTankOptions(): Promise<TankOptionsResponse> {
  const { data } = await api.get<TankOptionsResponse>(
    "/api/tanks/options"
  )
  return data
}

export async function calculateTankVolume(
  payload: TankVolumeRequest
): Promise<TankVolumeResponse> {
  const { data } = await api.post<TankVolumeResponse>(
    "/api/tanks/",
    payload
  )
  return data
}
