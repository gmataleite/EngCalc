import { z } from "zod"

export const shellThicknessFormSchema = z
  .object({
    unitSystem: z.enum(["metric", "imperial"]),
    shellType: z.enum(["cylindrical", "spherical"]),
    mode: z.enum(["min_thickness", "mawp"]),
    materialId: z.string().min(1, "Material specification is required."),
    designTemperature: z
      .number()
      .finite("Design temperature must be a valid number."),
    insideRadius: z
      .number()
      .positive("Inside radius must be greater than zero."),
    internalPressure: z
      .number()
      .positive("Internal pressure must be greater than zero."),
    jointTypeId: z.string().min(1, "Welding joint type is required."),
    rtId: z.string().min(1, "Radiographic examination is required."),
    corrosionAllowance: z
      .number()
      .min(0, "Corrosion allowance cannot be negative."),
    manualOverrideEnabled: z.boolean(),
    allowableStressOverride: z.number().optional(),
  })
  .superRefine((values, ctx) => {
    if (
      values.manualOverrideEnabled &&
      (values.allowableStressOverride === undefined ||
        Number.isNaN(values.allowableStressOverride) ||
        values.allowableStressOverride <= 0)
    ) {
      ctx.addIssue({
        code: "custom",
        path: ["allowableStressOverride"],
        message: "Manual allowable stress must be greater than zero.",
      })
    }
  })

export type ShellThicknessFormValues = z.infer<typeof shellThicknessFormSchema>

export const DEFAULT_SHELL_THICKNESS_VALUES: ShellThicknessFormValues = {
  unitSystem: "metric",
  shellType: "cylindrical",
  mode: "min_thickness",
  materialId: "SA-516-70",
  designTemperature: 40,
  insideRadius: 600,
  internalPressure: 1.0,
  jointTypeId: "type_1",
  rtId: "full",
  corrosionAllowance: 3.0,
  manualOverrideEnabled: false,
  allowableStressOverride: 138.0,
}
