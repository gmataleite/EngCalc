import { z } from "zod"

export const TankVolumeFormSchema = z
  .object({
    internalDiameter: z
      .number()
      .positive("Internal diameter must be greater than zero."),
    internalBodyHeight: z
      .number()
      .positive("Internal body height must be greater than zero."),
    bottomType: z.enum(["flat", "conical", "hemispherical", "ellipsoidal", "torispherical"]),
    bottomHeight: z
      .number()
      .positive("Bottom height must be greater than zero.")
      .nullable(),
    bottomAngle: z
      .number()
      .min(0, "Bottom angle must be greater than or equal to zero.")
      .max(90, "Bottom angle must be less than 90.")
      .nullable(),
    headType: z.enum(["flat", "conical", "hemispherical", "ellipsoidal", "torispherical"]),
    headHeight: z
      .number()
      .positive("Head height must be greater than zero.")
      .nullable(),
    headAngle: z
      .number()
      .min(0, "Head angle must be greater than or equal to zero.")
      .max(90, "Head angle must be less than 90.")
      .nullable(),
    productHeight: z
      .number()
      .positive("Product height must be greater than zero.")
      .nullable(),
    tankOrientation: z.enum(["horizontal", "vertical"]),
  })

export type TankVolumeFormValues = z.infer<typeof TankVolumeFormSchema>

export const DEFAULT_TANK_VOLUME_VALUES: TankVolumeFormValues = {
  internalDiameter: 1000,
  internalBodyHeight: 1000,
  bottomType: "conical",
  bottomHeight: null,
  bottomAngle: 45,
  headType: "conical",
  headHeight: null,
  headAngle: 15,
  productHeight: 1200,
  tankOrientation: "vertical",
}
