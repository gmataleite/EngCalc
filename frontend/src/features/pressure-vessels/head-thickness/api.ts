import { api } from "@/services/api"
import type {
  HeadThicknessCalculationRequest,
  HeadThicknessCalculationResponse,
  HeadThicknessOptionsResponse,
} from "./types"

export async function fetchHeadThicknessOptions(): Promise<HeadThicknessOptionsResponse> {
  const { data } = await api.get<HeadThicknessOptionsResponse>(
    "/api/calculations/pressure-vessels/head-thickness/options"
  )
  return data
}

export async function calculateHeadThickness(
  payload: HeadThicknessCalculationRequest
): Promise<HeadThicknessCalculationResponse> {
  const { data } = await api.post<HeadThicknessCalculationResponse>(
    "/api/calculations/pressure-vessels/head-thickness",
    payload
  )
  return data
}
