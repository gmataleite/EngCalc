import { z } from "zod"

export const headThicknessFormSchema = z
  .object({
    unitSystem: z.enum(["metric", "imperial"]),
    headType: z.enum([
      "ellipsoidal",
      "torispherical",
      "hemispherical",
      "conical",
    ]),
    mode: z.enum(["min_thickness", "mawp"]),
    materialId: z.string().min(1, "Material specification is required."),
    designTemperature: z
      .number()
      .finite("Design temperature must be a valid number."),
    internalPressure: z
      .number()
      .positive("Internal design pressure must be greater than zero."),
    insideDiameter: z
      .number()
      .positive("Inside diameter must be greater than zero."),
    crownRadius: z.number().positive("Crown radius must be greater than zero."),
    knuckleRadius: z
      .number()
      .positive("Knuckle radius must be greater than zero."),
    insideRadius: z
      .number()
      .positive("Inside radius must be greater than zero."),
    jointTypeId: z.string().min(1, "Welding joint description is required."),
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

    if (values.headType === "torispherical") {
      const ratio = values.knuckleRadius / values.crownRadius
      if (ratio < 0.06 || ratio > 0.5) {
        ctx.addIssue({
          code: "custom",
          path: ["knuckleRadius"],
          message:
            "Knuckle radius (r) must be between 6% and 50% of Crown Radius (L) per UG-32(d).",
        })
      }
    }
  })

export type HeadThicknessFormValues = z.infer<typeof headThicknessFormSchema>

export const DEFAULT_HEAD_THICKNESS_VALUES: HeadThicknessFormValues = {
  unitSystem: "metric",
  headType: "ellipsoidal",
  mode: "min_thickness",
  materialId: "SA-516-70",
  designTemperature: 40,
  internalPressure: 1.0,
  insideDiameter: 1200,
  crownRadius: 1200,
  knuckleRadius: 72,
  insideRadius: 600,
  jointTypeId: "type_1",
  rtId: "full",
  corrosionAllowance: 3.0,
  manualOverrideEnabled: false,
  allowableStressOverride: 138.0,
}
