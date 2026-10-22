import { api } from "@/services/api"
import { fetchMaterials } from "@/services/materials"
import type {
  ShellThicknessCalculationRequest,
  ShellThicknessCalculationResponse,
  ShellThicknessOptionsResponse,
} from "./types"

export { fetchMaterials }

export async function fetchShellThicknessOptions(): Promise<ShellThicknessOptionsResponse> {
  const { data } = await api.get<ShellThicknessOptionsResponse>(
    "/api/calculations/pressure-vessels/shell-thickness/options"
  )
  return data
}

export async function calculateShellThickness(
  payload: ShellThicknessCalculationRequest
): Promise<ShellThicknessCalculationResponse> {
  const { data } = await api.post<ShellThicknessCalculationResponse>(
    "/api/calculations/pressure-vessels/shell-thickness",
    payload
  )
  return data
}
