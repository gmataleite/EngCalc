import { api } from "@/services/api"
import type {
  MaterialOption,
  ShellThicknessCalculationRequest,
  ShellThicknessCalculationResponse,
  ShellThicknessOptionsResponse,
} from "./types"

export async function fetchMaterials(): Promise<MaterialOption[]> {
  const { data } = await api.get<MaterialOption[]>("/api/materials")
  return data
}

export async function fetchShellThicknessOptions(): Promise<ShellThicknessOptionsResponse> {
  const { data } = await api.get<ShellThicknessOptionsResponse>(
    "/api/calculations/shell-thickness/options"
  )
  return data
}

export async function calculateShellThickness(
  payload: ShellThicknessCalculationRequest
): Promise<ShellThicknessCalculationResponse> {
  const { data } = await api.post<ShellThicknessCalculationResponse>(
    "/api/calculations/shell-thickness",
    payload
  )
  return data
}
