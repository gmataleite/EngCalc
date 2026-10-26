import { z } from "zod"

export const ConverterFormSchema = z
  .object({
    physicalPropertyType: z.enum(["temperature", "pressure", "volume"]),
    initialValue: z.number().finite("Initial value must be a valid number."),
    initialUnit: z.string().min(1, "Initial unit is required."),
    targetUnit: z.string().min(1, "Target unit is required."),
  })
  .superRefine((values, ctx) => {
    if (values.initialUnit === values.targetUnit) {
      ctx.addIssue({
        code: "custom",
        path: ["targetUnit"],
        message: "Target unit must be different from initial unit.",
      })
    }
  })

export type ConverterFormValues = z.infer<typeof ConverterFormSchema>

export const DEFAULT_CONVERTER_VALUES: ConverterFormValues = {
  physicalPropertyType: "temperature",
  initialValue: 40,
  initialUnit: "°C",
  targetUnit: "°F",
}
