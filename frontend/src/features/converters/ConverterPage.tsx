import { useEffect, useMemo } from "react"
import { useForm, useWatch } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useMutation, useQuery } from "@tanstack/react-query"
import { AxiosError } from "axios"
import { ArrowLeftRight, AlertTriangle } from "lucide-react"

import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
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
import { cn } from "@/lib/utils"

import { calculateConverter, fetchConverterOptions } from "./api"
import { CONVERTER_TEXTS } from "./constants"
import {
  ConverterFormSchema,
  DEFAULT_CONVERTER_VALUES,
  type ConverterFormValues,
} from "./schema"
import type {
  PhysicalPropertyType,
  TemperatureUnit,
  PressureUnit,
  VolumeUnit,
  ConverterRequest,
} from "./types"

type ConverterUnit = TemperatureUnit | PressureUnit | VolumeUnit

interface UnitOption {
  id: ConverterUnit
  label: string
  backendValue: string
}

const TEMPERATURE_UNITS: UnitOption[] = [
  {
    id: "°C",
    label: CONVERTER_TEXTS.units.temperature.celsius,
    backendValue: "Celsius",
  },
  {
    id: "°F",
    label: CONVERTER_TEXTS.units.temperature.fahrenheit,
    backendValue: "Fahrenheit",
  },
  {
    id: "K",
    label: CONVERTER_TEXTS.units.temperature.kelvin,
    backendValue: "Kelvin",
  },
]

const PRESSURE_UNITS: UnitOption[] = [
  {
    id: "Pa",
    label: CONVERTER_TEXTS.units.pressure.pascal,
    backendValue: "Pascal",
  },
  {
    id: "bar",
    label: CONVERTER_TEXTS.units.pressure.bar,
    backendValue: "Bar",
  },
  {
    id: "psi",
    label: CONVERTER_TEXTS.units.pressure.psi,
    backendValue: "PoundsPerSquareInch",
  },
  {
    id: "N/m²",
    label: CONVERTER_TEXTS.units.pressure.newtonPerSquareMeter,
    backendValue: "NewtonPerSquareMeter",
  },
]

const VOLUME_UNITS: UnitOption[] = [
  {
    id: "m3",
    label: CONVERTER_TEXTS.units.volume.cubicMeters,
    backendValue: "CubicMeters",
  },
  {
    id: "L",
    label: CONVERTER_TEXTS.units.volume.liters,
    backendValue: "Liters",
  },
  {
    id: "gal",
    label: CONVERTER_TEXTS.units.volume.gallons,
    backendValue: "Gallons",
  },
]

const UNITS_BY_PROPERTY: Record<PhysicalPropertyType, UnitOption[]> = {
  temperature: TEMPERATURE_UNITS,
  pressure: PRESSURE_UNITS,
  volume: VOLUME_UNITS,
}

const DEFAULT_UNITS_BY_PROPERTY: Record<
  PhysicalPropertyType,
  { initialUnit: ConverterUnit; targetUnit: ConverterUnit }
> = {
  temperature: { initialUnit: "°C", targetUnit: "°F" },
  pressure: { initialUnit: "Pa", targetUnit: "bar" },
  volume: { initialUnit: "m3", targetUnit: "L" },
}

const UNIT_TO_BACKEND: Record<string, string> = {
  "°C": "Celsius",
  "°F": "Fahrenheit",
  K: "Kelvin",
  celsius: "Celsius",
  fahrenheit: "Fahrenheit",
  kelvin: "Kelvin",
  Pa: "Pascal",
  bar: "Bar",
  psi: "PoundsPerSquareInch",
  "N/m²": "NewtonPerSquareMeter",
  pascal: "Pascal",
  poundsPerSquareInch: "PoundsPerSquareInch",
  newtonPerSquareMeter: "NewtonPerSquareMeter",
  m3: "CubicMeters",
  L: "Liters",
  gal: "Gallons",
  cubicMeters: "CubicMeters",
  liters: "Liters",
  gallons: "Gallons",
}

export default function ConverterPage() {
  const form = useForm<ConverterFormValues>({
    resolver: zodResolver(ConverterFormSchema),
    defaultValues: DEFAULT_CONVERTER_VALUES,
    mode: "onChange",
  })

  const watchedValues = useWatch({
    control: form.control,
  })

  const debouncedValues = useDebounce(watchedValues, 300)

  const optionsQuery = useQuery({
    queryKey: ["converter-options"],
    queryFn: fetchConverterOptions,
  })

  const propertyOptions = useMemo(() => {
    if (optionsQuery.data?.PhysicalPropertiesTypes?.length) {
      return optionsQuery.data.PhysicalPropertiesTypes
    }
    return [
      {
        id: "temperature",
        label:
          CONVERTER_TEXTS.topConfig.physicalPropertyTypeOptions.temperature,
      },
      {
        id: "pressure",
        label: CONVERTER_TEXTS.topConfig.physicalPropertyTypeOptions.pressure,
      },
      {
        id: "volume",
        label: CONVERTER_TEXTS.topConfig.physicalPropertyTypeOptions.volume,
      },
    ]
  }, [optionsQuery.data])

  const calculationMutation = useMutation({
    mutationFn: calculateConverter,
  })

  // Trigger conversion calculation automatically when debounced values are valid
  const mutateCalculation = calculationMutation.mutate
  useEffect(() => {
    const parsed = ConverterFormSchema.safeParse(debouncedValues)
    if (!parsed.success) return

    const v = parsed.data
    const validUnits = UNITS_BY_PROPERTY[v.physicalPropertyType]
    if (
      !validUnits?.some((u) => u.id === v.initialUnit) ||
      !validUnits?.some((u) => u.id === v.targetUnit)
    ) {
      return
    }

    const backendInitial = (UNIT_TO_BACKEND[v.initialUnit] ??
      v.initialUnit) as ConverterUnit
    const backendTarget = (UNIT_TO_BACKEND[v.targetUnit] ??
      v.targetUnit) as ConverterUnit

    const payload: ConverterRequest = {
      physicalPropertyType: v.physicalPropertyType,
      initialValue: v.initialValue,
      initialUnit: backendInitial,
      targetUnit: backendTarget,
    }

    mutateCalculation(payload)
  }, [debouncedValues, mutateCalculation])

  const result = calculationMutation.data

  const apiErrorMessage = useMemo(() => {
    if (!calculationMutation.error) return null
    if (calculationMutation.error instanceof AxiosError) {
      const responseData = calculationMutation.error.response?.data as
        { message?: string } | undefined
      return (
        responseData?.message ??
        calculationMutation.error.message ??
        "Unable to calculate conversion."
      )
    }
    return calculationMutation.error instanceof Error
      ? calculationMutation.error.message
      : "Unable to calculate conversion."
  }, [calculationMutation.error])

  const physicalPropertyType =
    watchedValues.physicalPropertyType ??
    DEFAULT_CONVERTER_VALUES.physicalPropertyType
  const availableUnits =
    UNITS_BY_PROPERTY[physicalPropertyType] ?? TEMPERATURE_UNITS

  const initialUnit = watchedValues.initialUnit
  const targetUnit = watchedValues.targetUnit
  const initialValue = watchedValues.initialValue

  const hasSameUnits = initialUnit === targetUnit

  const handleSwapUnits = () => {
    const currentInitial = form.getValues("initialUnit")
    const currentTarget = form.getValues("targetUnit")
    form.setValue("initialUnit", currentTarget, { shouldValidate: true })
    form.setValue("targetUnit", currentInitial, { shouldValidate: true })
  }

  const formattedConvertedValue = useMemo(() => {
    if (calculationMutation.isPending && !result) {
      return "..."
    }
    if (result && typeof result.convertedValue === "number") {
      return `${result.convertedValue.toLocaleString("en-US", {
        maximumFractionDigits: 4,
      })} ${targetUnit ?? ""}`
    }
    return `— ${targetUnit ?? ""}`
  }, [result, calculationMutation.isPending, targetUnit])

  const conversionSubtext = useMemo(() => {
    if (result && typeof result.convertedValue === "number" && !hasSameUnits) {
      return `${initialValue} ${initialUnit} = ${result.convertedValue} ${targetUnit}`
    }
    return undefined
  }, [result, initialValue, initialUnit, targetUnit, hasSameUnits])

  const currentFormulaReference =
    CONVERTER_TEXTS.formulaReference[physicalPropertyType] ??
    CONVERTER_TEXTS.formulaReference.temperature

  return (
    <div className="mx-auto max-w-6xl space-y-6 pb-12">
      {/* Page Header & Physical Property Selection */}
      <section className="space-y-4">
        <div className="space-y-1">
          <h1 className="text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
            {CONVERTER_TEXTS.pageTitle}
          </h1>
          <p className="text-sm text-muted-foreground">
            {CONVERTER_TEXTS.pageSubtitle}
          </p>
        </div>

        <div className="space-y-2 pt-2">
          <Label className="text-xs font-medium text-muted-foreground">
            {CONVERTER_TEXTS.topConfig.physicalPropertyLabel}
          </Label>
          <RadioGroup
            value={watchedValues.physicalPropertyType}
            onValueChange={(val) => {
              const nextProperty = val as PhysicalPropertyType
              if (nextProperty in UNITS_BY_PROPERTY) {
                const defaultUnits = DEFAULT_UNITS_BY_PROPERTY[nextProperty]
                form.setValue("physicalPropertyType", nextProperty, {
                  shouldValidate: true,
                })
                form.setValue("initialUnit", defaultUnits.initialUnit, {
                  shouldValidate: true,
                })
                form.setValue("targetUnit", defaultUnits.targetUnit, {
                  shouldValidate: true,
                })
              }
            }}
            className="flex flex-wrap items-center gap-4 pt-1"
          >
            {propertyOptions.map((opt) => {
              const isSupported =
                opt.id === "temperature" ||
                opt.id === "pressure" ||
                opt.id === "volume"
              return (
                <label
                  key={opt.id}
                  className={cn(
                    "flex items-center gap-2 text-xs",
                    isSupported
                      ? "cursor-pointer text-foreground"
                      : "cursor-not-allowed text-muted-foreground opacity-60"
                  )}
                >
                  <RadioGroupItem value={opt.id} disabled={!isSupported} />
                  <span>
                    {opt.label}
                    {!isSupported && " (Planned)"}
                  </span>
                </label>
              )
            })}
          </RadioGroup>
        </div>
      </section>

      <Separator />

      {/* Main Conversion Card */}
      <Card>
        <CardHeader>
          <CardTitle className="text-base font-semibold">
            {CONVERTER_TEXTS.converter.cardTitle}
          </CardTitle>
          <CardDescription className="text-xs">
            {CONVERTER_TEXTS.converter.cardDescription}
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          {/* Stepper, Unit Selection & Result Stack */}
          <div className="space-y-4">
            {/* 1. Initial Value */}
            <NumberStepperField
              id="initialValue"
              label={CONVERTER_TEXTS.converter.initialValueLabel(initialUnit)}
              value={initialValue ?? 0}
              step={1}
              error={form.formState.errors.initialValue?.message}
              onChange={(next) =>
                form.setValue("initialValue", next, { shouldValidate: true })
              }
            />

            {/* 2. From Unit + Swap Button inline */}
            <div className="flex flex-col gap-1.5">
              <Label className="text-xs font-normal text-muted-foreground">
                {CONVERTER_TEXTS.converter.initialUnitLabel}
              </Label>
              <div className="flex items-center gap-2">
                <Select
                  value={initialUnit}
                  onValueChange={(val) => {
                    if (val) {
                      form.setValue("initialUnit", val, {
                        shouldValidate: true,
                      })
                    }
                  }}
                >
                  <SelectTrigger className="h-9 flex-1 bg-muted/40 px-3 text-xs data-[size=default]:h-9">
                    <SelectValue
                      placeholder={
                        CONVERTER_TEXTS.converter.initialUnitPlaceholder
                      }
                    />
                  </SelectTrigger>
                  <SelectContent>
                    {availableUnits.map((u) => (
                      <SelectItem key={u.id} value={u.id}>
                        {u.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>

                <Button
                  type="button"
                  variant="outline"
                  size="icon"
                  onClick={handleSwapUnits}
                  title={CONVERTER_TEXTS.converter.swapUnitsTooltip}
                  className="size-9 shrink-0 rounded-md bg-muted/40"
                >
                  <ArrowLeftRight className="size-4" />
                  <span className="sr-only">
                    {CONVERTER_TEXTS.converter.swapUnitsTooltip}
                  </span>
                </Button>
              </div>
            </div>

            {/* 3. To Unit */}
            <div className="flex flex-col gap-1.5">
              <Label className="text-xs font-normal text-muted-foreground">
                {CONVERTER_TEXTS.converter.targetUnitLabel}
              </Label>
              <Select
                value={targetUnit}
                onValueChange={(val) => {
                  if (val) {
                    form.setValue("targetUnit", val, { shouldValidate: true })
                  }
                }}
              >
                <SelectTrigger className="h-9 w-full bg-muted/40 px-3 text-xs data-[size=default]:h-9">
                  <SelectValue
                    placeholder={
                      CONVERTER_TEXTS.converter.targetUnitPlaceholder
                    }
                  />
                </SelectTrigger>
                <SelectContent>
                  {availableUnits.map((u) => (
                    <SelectItem key={u.id} value={u.id}>
                      {u.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* 4. Converted Value */}
            <div className="flex min-h-[58px] items-center justify-between rounded-md border border-input bg-muted/20 px-4 py-2">
              <MetricCard
                label={CONVERTER_TEXTS.converter.convertedValueCardLabel}
                value={formattedConvertedValue}
                subtext={conversionSubtext}
              />
              {calculationMutation.isPending && (
                <span className="animate-pulse text-xs text-muted-foreground">
                  {CONVERTER_TEXTS.converter.calculatingMessage}
                </span>
              )}
            </div>
          </div>

          {/* Validation Warnings / Error Alerts */}
          {hasSameUnits && (
            <Alert className="border-amber-300 bg-amber-50/80 text-amber-900 dark:border-amber-900/50 dark:bg-amber-950/30 dark:text-amber-200">
              <AlertTriangle className="size-4 text-amber-700 dark:text-amber-300" />
              <AlertDescription className="text-xs text-amber-800 dark:text-amber-200">
                {CONVERTER_TEXTS.converter.sameUnitWarning}
              </AlertDescription>
            </Alert>
          )}

          {apiErrorMessage && (
            <Alert variant="destructive">
              <AlertTriangle className="size-4" />
              <AlertDescription className="text-xs">
                {apiErrorMessage}
              </AlertDescription>
            </Alert>
          )}
        </CardContent>
      </Card>

      <Separator />

      {/* Formula Reference */}
      <section className="space-y-4">
        <h3 className="text-base font-semibold text-foreground">
          {currentFormulaReference.title}
        </h3>

        <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
          {currentFormulaReference.items.map((item) => {
            const isActive = item.from === initialUnit && item.to === targetUnit
            return (
              <div
                key={item.title}
                className={cn(
                  "space-y-2 rounded-lg border p-4 transition-colors",
                  isActive
                    ? "border-primary/50 bg-primary/5 shadow-xs"
                    : "border-border bg-card/50"
                )}
              >
                <div className="flex items-center justify-between">
                  <h4
                    className={cn(
                      "text-xs font-semibold",
                      isActive ? "text-primary" : "text-foreground"
                    )}
                  >
                    {item.title}
                  </h4>
                  {isActive && (
                    <span className="rounded-full bg-primary/10 px-2 py-0.5 text-[10px] font-medium text-primary">
                      Active
                    </span>
                  )}
                </div>
                <div className="rounded-md bg-muted/20 p-3">
                  <Latex formula={item.latex} displayMode />
                </div>
              </div>
            )
          })}
        </div>
      </section>
    </div>
  )
}
