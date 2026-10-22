import { api } from "@/services/api"
import type { MaterialOption } from "@/types/common"

export async function fetchMaterials(): Promise<MaterialOption[]> {
  const { data } = await api.get<MaterialOption[]>("/api/materials")
  return data
}
