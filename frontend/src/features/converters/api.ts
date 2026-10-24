import { api } from "@/services/api"
import type {
  ConverterRequest,
  ConverterResponse,
  ConverterOptionsResponse,
} from "./types"

export async function fetchConverterOptions(): Promise<ConverterOptionsResponse> {
  const { data } = await api.get<ConverterOptionsResponse>(
    "/api/calculations/converters/options"
  )
  return data
}

export async function calculateConverter(
  payload: ConverterRequest
): Promise<ConverterResponse> {
  const { data } = await api.post<ConverterResponse>(
    "/api/calculations/converters",
    payload
  )
  return data
}
