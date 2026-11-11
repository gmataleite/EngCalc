import { useEffect, useMemo, useState } from "react"
import { useForm, useWatch } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useMutation, useQuery } from "@tanstack/react-query"
import { AxiosError } from "axios"
import { AlertTriangle } from "lucide-react"

import { Alert, AlertDescription } from "@/components/ui/alert"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Separator } from "@/components/ui/separator"
import { Latex } from "@/components/shared/Latex"
import { MetricCard } from "@/components/shared/MetricCard"
import { NumberStepperField } from "@/components/shared/NumberStepperField"
import { useDebounce } from "@/hooks/useDebounce"

import { calculateTankVolume, fetchTankOptions } from "./api"
import {
  DEFAULT_TANK_OPTIONS,
  TANK_VOLUME_TEXTS,
  type TankDimensionMode,
} from "./constants"
import {
  TankVolumeFormSchema,
  DEFAULT_TANK_VALUES,
  type TankVolumeFormValues,
} from "./schema"
import type {
  TankHeadTypes,
  TankOrientationTypes,
  TankVolumeRequest,
} from "./types"

const ORIENTATIONS: TankOrientationTypes[] = ["horizontal", "vertical"]

function getErrorMessage(error: unknown, fallback: string) {
  if (error instanceof AxiosError) {
    const responseData = error.response?.data as
      { message?: string } | undefined
    return responseData?.message ?? error.message ?? fallback
  }
  return error instanceof Error ? error.message : fallback
}

function optionLabel(value: string) {
  return value
    .replace(/([a-z])([A-Z])/g, "$1 $2")
    .replace(/^./, (character) => character.toUpperCase())
}

export default function TankVolumePage() {
  const [bottomDimensionMode, setBottomDimensionMode] =
    useState<TankDimensionMode>("angle")
  const [headDimensionMode, setHeadDimensionMode] =
    useState<TankDimensionMode>("angle")

  const form = useForm<TankVolumeFormValues>({
    resolver: zodResolver(TankVolumeFormSchema),
    defaultValues: DEFAULT_TANK_VALUES,
    mode: "onChange",
  })

  const watchedValues = useWatch({ control: form.control })
  const debouncedValues = useDebounce(watchedValues, 300)

  const optionsQuery = useQuery({
    queryKey: ["tank-volume-options"],
    queryFn: fetchTankOptions,
  })

  const calculationMutation = useMutation({
    mutationFn: calculateTankVolume,
  })

  const mutateCalculation = calculationMutation.mutate
  const resetCalculation = calculationMutation.reset

  useEffect(() => {
    const parsed = TankVolumeFormSchema.safeParse(debouncedValues)
    if (!parsed.success) {
      resetCalculation()
      return
    }

    const values = parsed.data
    const payload: TankVolumeRequest = {
      internalDiameter: values.internalDiameter,
      internalBodyHeight: values.internalBodyHeight,
      bottomType: values.bottomType,
      bottomHeight: values.bottomHeight,
      bottomAngle: values.bottomAngle,
      headType: values.headType,
      headHeight: values.headHeight,
      headAngle: values.headAngle,
      productHeight: values.productHeight,
      tankOrientation: values.tankOrientation,
    }

    mutateCalculation(payload)
  }, [debouncedValues, mutateCalculation, resetCalculation])

  const headOptions = useMemo(() => {
    const options = optionsQuery.data?.tankHeadTypes
    return (options?.length ? options : DEFAULT_TANK_OPTIONS.headTypes).map(
      (option) => ({ ...option, id: option.id.toLowerCase() })
    )
  }, [optionsQuery.data])

  const orientationOptions = useMemo(() => {
    const options = optionsQuery.data?.tankOrientationTypes
    return options?.length ? options : DEFAULT_TANK_OPTIONS.orientations
  }, [optionsQuery.data])

  const selectedBottomType =
    watchedValues.bottomType ?? DEFAULT_TANK_VALUES.bottomType
  const selectedHeadType =
    watchedValues.headType ?? DEFAULT_TANK_VALUES.headType
  const selectedOrientation =
    watchedValues.tankOrientation ?? DEFAULT_TANK_VALUES.tankOrientation
  const result = calculationMutation.data
  const apiErrorMessage = calculationMutation.error
    ? getErrorMessage(
        calculationMutation.error,
        "Unable to calculate tank volume."
      )
    : null

  const setDimensionMode = (
    type: "bottom" | "head",
    mode: TankDimensionMode
  ) => {
    if (type === "bottom") {
      setBottomDimensionMode(mode)
      form.setValue("bottomHeight", mode === "height" ? 500 : null, {
        shouldValidate: true,
      })
      form.setValue("bottomAngle", mode === "angle" ? 45 : null, {
        shouldValidate: true,
      })
    } else {
      setHeadDimensionMode(mode)
      form.setValue("headHeight", mode === "height" ? 200 : null, {
        shouldValidate: true,
      })
      form.setValue("headAngle", mode === "angle" ? 15 : null, {
        shouldValidate: true,
      })
    }
  }

  const formatVolume = (value: number | undefined) =>
    value === undefined
      ? "— mm³"
      : `${value.toLocaleString("en-US", { maximumFractionDigits: 4 })} mm³`

  const fieldError = (field: keyof TankVolumeFormValues) =>
    form.formState.errors[field]?.message

  return (
    <div className="mx-auto max-w-6xl space-y-6 pb-12">
      <section className="space-y-1">
        <h1 className="text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
          {TANK_VOLUME_TEXTS.pageTitle}
        </h1>
        <p className="text-sm text-muted-foreground">
          {TANK_VOLUME_TEXTS.pageSubtitle}
        </p>
      </section>

      <Card>
        <CardHeader>
          <CardTitle className="text-base font-semibold">
            {TANK_VOLUME_TEXTS.configuration.title}
          </CardTitle>
          <CardDescription className="text-xs">
            {TANK_VOLUME_TEXTS.configuration.description}
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <NumberStepperField
              id="internalDiameter"
              label={TANK_VOLUME_TEXTS.fields.internalDiameter}
              value={watchedValues.internalDiameter ?? 0}
              min={0}
              error={fieldError("internalDiameter")}
              onChange={(value) =>
                form.setValue("internalDiameter", value, {
                  shouldValidate: true,
                })
              }
            />
            <NumberStepperField
              id="internalBodyHeight"
              label={TANK_VOLUME_TEXTS.fields.internalBodyHeight}
              value={watchedValues.internalBodyHeight ?? 0}
              min={0}
              error={fieldError("internalBodyHeight")}
              onChange={(value) =>
                form.setValue("internalBodyHeight", value, {
                  shouldValidate: true,
                })
              }
            />
          </div>

          <div className="space-y-2">
            <Label className="text-xs font-medium text-muted-foreground">
              {TANK_VOLUME_TEXTS.fields.orientation}
            </Label>
            <RadioGroup
              value={selectedOrientation.toLowerCase()}
              onValueChange={(value) => {
                const orientation = value.toLowerCase() as TankOrientationTypes
                if (ORIENTATIONS.includes(orientation)) {
                  form.setValue("tankOrientation", orientation, {
                    shouldValidate: true,
                  })
                }
              }}
              className="flex flex-wrap gap-4"
            >
              {orientationOptions.map((option) => (
                <label
                  key={option.id}
                  className="flex cursor-pointer items-center gap-2 text-xs text-foreground"
                >
                  <RadioGroupItem value={option.id.toLowerCase()} />
                  <span>{option.label}</span>
                </label>
              ))}
            </RadioGroup>
          </div>

          <Separator />

          <div className="grid grid-cols-1 gap-6 lg:grid-cols-2">
            <TankHeadSection
              title={TANK_VOLUME_TEXTS.bottom.title}
              type={selectedBottomType}
              options={headOptions}
              dimensionMode={bottomDimensionMode}
              height={watchedValues.bottomHeight}
              angle={watchedValues.bottomAngle}
              heightError={fieldError("bottomHeight")}
              angleError={fieldError("bottomAngle")}
              onTypeChange={(value) =>
                form.setValue("bottomType", value, { shouldValidate: true })
              }
              onModeChange={(mode) => setDimensionMode("bottom", mode)}
              onHeightChange={(value) =>
                form.setValue("bottomHeight", value, { shouldValidate: true })
              }
              onAngleChange={(value) =>
                form.setValue("bottomAngle", value, { shouldValidate: true })
              }
              fields={TANK_VOLUME_TEXTS.bottom}
            />
            <TankHeadSection
              title={TANK_VOLUME_TEXTS.head.title}
              type={selectedHeadType}
              options={headOptions}
              dimensionMode={headDimensionMode}
              height={watchedValues.headHeight}
              angle={watchedValues.headAngle}
              heightError={fieldError("headHeight")}
              angleError={fieldError("headAngle")}
              onTypeChange={(value) =>
                form.setValue("headType", value, { shouldValidate: true })
              }
              onModeChange={(mode) => setDimensionMode("head", mode)}
              onHeightChange={(value) =>
                form.setValue("headHeight", value, { shouldValidate: true })
              }
              onAngleChange={(value) =>
                form.setValue("headAngle", value, { shouldValidate: true })
              }
              fields={TANK_VOLUME_TEXTS.head}
            />
          </div>

          <NumberStepperField
            id="productHeight"
            label={TANK_VOLUME_TEXTS.fields.productHeight}
            value={watchedValues.productHeight ?? 0}
            min={0}
            error={fieldError("productHeight")}
            onChange={(value) =>
              form.setValue("productHeight", value, { shouldValidate: true })
            }
          />

          {calculationMutation.isPending && (
            <p className="text-xs text-muted-foreground">
              {TANK_VOLUME_TEXTS.results.calculating}
            </p>
          )}
          {apiErrorMessage && (
            <Alert variant="destructive">
              <AlertTriangle className="size-4" />
              <AlertDescription>{apiErrorMessage}</AlertDescription>
            </Alert>
          )}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base font-semibold">
            {TANK_VOLUME_TEXTS.results.title}
          </CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <MetricCard
            label={TANK_VOLUME_TEXTS.results.internalVolume}
            value={formatVolume(result?.internalVolume)}
          />
          <MetricCard
            label={TANK_VOLUME_TEXTS.results.productVolume}
            value={formatVolume(result?.productVolume)}
          />
        </CardContent>
      </Card>

      <Separator />

      <section className="space-y-4">
        <h3 className="text-base font-semibold text-foreground">
          {TANK_VOLUME_TEXTS.formulaReference.title}
        </h3>
        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
          {TANK_VOLUME_TEXTS.formulaReference.items.map((item) => (
            <div key={item.title} className="space-y-2 rounded-lg border p-4">
              <h4 className="text-xs font-semibold text-foreground">
                {item.title}
              </h4>
              <div className="rounded-md bg-muted/20 p-3">
                <Latex formula={item.latex} />
              </div>
            </div>
          ))}
        </div>
      </section>
    </div>
  )
}

interface TankHeadSectionProps {
  title: string
  type: TankHeadTypes
  options: ReadonlyArray<{ id: string; label: string }>
  dimensionMode: TankDimensionMode
  height: number | null | undefined
  angle: number | null | undefined
  heightError?: string
  angleError?: string
  onTypeChange: (value: TankHeadTypes) => void
  onModeChange: (mode: TankDimensionMode) => void
  onHeightChange: (value: number) => void
  onAngleChange: (value: number) => void
  fields: {
    typeLabel: string
    dimensionLabel: string
    heightLabel: string
    angleLabel: string
  }
}

function TankHeadSection({
  title,
  type,
  options,
  dimensionMode,
  height,
  angle,
  heightError,
  angleError,
  onTypeChange,
  onModeChange,
  onHeightChange,
  onAngleChange,
  fields,
}: TankHeadSectionProps) {
  const isConical = type.toLowerCase() === "conical"
  const usesHeight = dimensionMode === "height"

  return (
    <div className="space-y-4 rounded-lg border p-4">
      <h3 className="text-sm font-semibold text-foreground">{title}</h3>
      <div className="space-y-1.5">
        <Label className="text-xs font-normal text-muted-foreground">
          {fields.typeLabel}
        </Label>
        <Select
          value={type}
          onValueChange={(value) => onTypeChange(value as TankHeadTypes)}
        >
          <SelectTrigger className="h-9 bg-muted/40 px-3 text-xs">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            {options.map((option) => (
              <SelectItem key={option.id} value={option.id}>
                {option.label || optionLabel(option.id)}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {isConical ? (
        <div className="space-y-2">
          <Label className="text-xs font-normal text-muted-foreground">
            {fields.dimensionLabel}
          </Label>
          <RadioGroup
            value={dimensionMode}
            onValueChange={(value) => {
              if (value === "height" || value === "angle") onModeChange(value)
            }}
            className="flex gap-4"
          >
            <label className="flex cursor-pointer items-center gap-2 text-xs">
              <RadioGroupItem value="height" />
              <span>{fields.heightLabel}</span>
            </label>
            <label className="flex cursor-pointer items-center gap-2 text-xs">
              <RadioGroupItem value="angle" />
              <span>{fields.angleLabel}</span>
            </label>
          </RadioGroup>
          {usesHeight ? (
            <NumberStepperField
              id={`${title}-height`}
              label={fields.heightLabel}
              value={height ?? 0}
              min={0}
              error={heightError}
              onChange={onHeightChange}
            />
          ) : (
            <NumberStepperField
              id={`${title}-angle`}
              label={fields.angleLabel}
              value={angle ?? 0}
              min={0}
              error={angleError}
              onChange={onAngleChange}
            />
          )}
        </div>
      ) : null}
    </div>
  )
}
