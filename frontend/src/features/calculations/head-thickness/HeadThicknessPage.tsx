import { useEffect, useMemo, useState } from "react"
import { useForm, useWatch } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useMutation, useQuery } from "@tanstack/react-query"
import { AxiosError } from "axios"
import { Check, ChevronsUpDown, AlertTriangle, Pin } from "lucide-react"

import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command"
import { Label } from "@/components/ui/label"
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover"
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
import { fetchMaterials } from "@/features/calculations/shell-thickness/api"
import type {
  CalculationMode,
  UnitSystem,
} from "@/features/calculations/shell-thickness/types"
import { useDebounce } from "@/hooks/useDebounce"
import { cn } from "@/lib/utils"

import { calculateHeadThickness, fetchHeadThicknessOptions } from "./api"
import { HEAD_THICKNESS_TEXTS } from "./constants"
import {
  DEFAULT_HEAD_THICKNESS_VALUES,
  headThicknessFormSchema,
  type HeadThicknessFormValues,
} from "./schema"
import type { HeadThicknessCalculationRequest, HeadType } from "./types"

export default function HeadThicknessPage() {
  const [materialPopoverOpen, setMaterialPopoverOpen] = useState(false)

  const form = useForm<HeadThicknessFormValues>({
    resolver: zodResolver(headThicknessFormSchema),
    defaultValues: DEFAULT_HEAD_THICKNESS_VALUES,
    mode: "onChange",
  })

  const watchedValues = useWatch({
    control: form.control,
  })

  const debouncedValues = useDebounce(watchedValues, 300)

  const materialsQuery = useQuery({
    queryKey: ["materials"],
    queryFn: fetchMaterials,
  })

  const optionsQuery = useQuery({
    queryKey: ["head-thickness-options"],
    queryFn: fetchHeadThicknessOptions,
  })

  const calculationMutation = useMutation({
    mutationFn: calculateHeadThickness,
  })

  const unitSystem: UnitSystem = watchedValues.unitSystem ?? "metric"
  const headType: HeadType = watchedValues.headType ?? "ellipsoidal"
  const activeUnits = HEAD_THICKNESS_TEXTS.units[unitSystem]

  // Ensure valid initial materialId when materials load from the backend
  useEffect(() => {
    const materials = materialsQuery.data
    if (!materials || materials.length === 0) return

    const currentId = form.getValues("materialId")
    const exists = materials.some((m) => m.id === currentId)
    if (!exists) {
      form.setValue("materialId", materials[0].id, { shouldValidate: true })
    }
  }, [materialsQuery.data, form])

  // Trigger calculation automatically when debounced form inputs are valid
  const mutateCalculation = calculationMutation.mutate
  useEffect(() => {
    const parsed = headThicknessFormSchema.safeParse(debouncedValues)
    if (!parsed.success) return

    const v = parsed.data
    const payload: HeadThicknessCalculationRequest = {
      unitSystem: v.unitSystem,
      headType: v.headType,
      mode: v.mode,
      materialId: v.materialId,
      designTemperature: v.designTemperature,
      internalPressure: v.internalPressure,
      insideDiameter: v.insideDiameter,
      crownRadius: v.crownRadius,
      knuckleRadius: v.knuckleRadius,
      insideRadius: v.insideRadius,
      jointTypeId: v.jointTypeId,
      rtId: v.rtId,
      corrosionAllowance: v.corrosionAllowance,
      allowableStressOverride: v.manualOverrideEnabled
        ? v.allowableStressOverride
        : undefined,
    }

    mutateCalculation(payload)
  }, [debouncedValues, mutateCalculation])

  const selectedMaterial = useMemo(() => {
    return materialsQuery.data?.find((m) => m.id === watchedValues.materialId)
  }, [materialsQuery.data, watchedValues.materialId])

  const selectedJointTypeLabel = useMemo(() => {
    return (
      optionsQuery.data?.jointTypes.find(
        (j) => j.id === watchedValues.jointTypeId
      )?.label ?? watchedValues.jointTypeId
    )
  }, [optionsQuery.data, watchedValues.jointTypeId])

  const selectedRtLabel = useMemo(() => {
    return (
      optionsQuery.data?.rtOptions.find((r) => r.id === watchedValues.rtId)
        ?.label ?? watchedValues.rtId
    )
  }, [optionsQuery.data, watchedValues.rtId])

  const result = calculationMutation.data

  const apiErrorMessage = useMemo(() => {
    if (!calculationMutation.error) return null
    if (calculationMutation.error instanceof AxiosError) {
      const responseData = calculationMutation.error.response?.data as
        { message?: string } | undefined
      return (
        responseData?.message ??
        calculationMutation.error.message ??
        "Unable to compute head thickness."
      )
    }
    return calculationMutation.error instanceof Error
      ? calculationMutation.error.message
      : "Unable to compute head thickness."
  }, [calculationMutation.error])

  const formattedAllowableStress = result
    ? `${result.allowableStress.toLocaleString("en-US", {
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      })} ${result.pressureUnit}`
    : `— ${activeUnits.pressure}`

  const formattedJointEfficiency = result
    ? `${result.jointEfficiency.toFixed(2)} (${result.jointEfficiencyPercent.toFixed(0)}%)`
    : "—"

  const formattedTotalThickness = result
    ? `${result.totalMinimumThickness.toFixed(4)} ${result.lengthUnit}`
    : `— ${activeUnits.length}`

  const fallbackActiveLogic = useMemo(() => {
    switch (headType) {
      case "torispherical":
        return "Torispherical Heads (UG-32(d) / Appendix 1-4(d))"
      case "hemispherical":
        return "Hemispherical Heads (UG-32(e) / Appendix 1-3)"
      default:
        return "Ellipsoidal Heads (UG-32(c))"
    }
  }, [headType])

  const fallbackReferenceLatex = useMemo(() => {
    switch (headType) {
      case "torispherical":
        return String.raw`t = \frac{P \cdot L \cdot M}{2SE - 0.2P} + CA`
      case "hemispherical":
        return String.raw`t = \frac{P \cdot L}{2SE - 0.2P} + CA`
      default:
        return String.raw`t = \frac{P \cdot D}{2SE - 0.2P} + CA`
    }
  }, [headType])

  return (
    <div className="mx-auto max-w-6xl space-y-6 pb-12">
      {/* Page Header & Top Configuration */}
      <section className="space-y-4">
        <div className="space-y-1">
          <h1 className="text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
            {HEAD_THICKNESS_TEXTS.pageTitle}
          </h1>
          <h2 className="text-base font-semibold text-foreground/90 sm:text-lg">
            {HEAD_THICKNESS_TEXTS.pageSubtitle}
          </h2>
        </div>

        <div className="grid grid-cols-1 gap-6 pt-2 md:grid-cols-2">
          {/* Unit System */}
          <div className="space-y-2">
            <Label className="text-xs font-medium text-muted-foreground">
              {HEAD_THICKNESS_TEXTS.topConfig.unitSystemLabel}
            </Label>
            <RadioGroup
              value={watchedValues.unitSystem}
              onValueChange={(val) => {
                if (val === "metric" || val === "imperial") {
                  form.setValue("unitSystem", val as UnitSystem, {
                    shouldValidate: true,
                  })
                }
              }}
              className="flex flex-wrap items-center gap-4 pt-1"
            >
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="imperial" />
                <span>
                  {HEAD_THICKNESS_TEXTS.topConfig.unitOptions.imperial}
                </span>
              </label>
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="metric" />
                <span>{HEAD_THICKNESS_TEXTS.topConfig.unitOptions.metric}</span>
              </label>
            </RadioGroup>
          </div>

          {/* Formed Head Type */}
          <div className="space-y-2">
            <Label className="text-xs font-medium text-muted-foreground">
              {HEAD_THICKNESS_TEXTS.topConfig.headTypeLabel}
            </Label>
            <RadioGroup
              value={watchedValues.headType}
              onValueChange={(val) => {
                if (
                  val === "ellipsoidal" ||
                  val === "torispherical" ||
                  val === "hemispherical"
                ) {
                  form.setValue("headType", val as HeadType, {
                    shouldValidate: true,
                  })
                }
              }}
              className="flex flex-wrap items-center gap-4 pt-1"
            >
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="ellipsoidal" />
                <span>
                  {HEAD_THICKNESS_TEXTS.topConfig.headTypeOptions.ellipsoidal}
                </span>
              </label>
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="torispherical" />
                <span>
                  {HEAD_THICKNESS_TEXTS.topConfig.headTypeOptions.torispherical}
                </span>
              </label>
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="hemispherical" />
                <span>
                  {HEAD_THICKNESS_TEXTS.topConfig.headTypeOptions.hemispherical}
                </span>
              </label>
              <label className="flex cursor-not-allowed items-center gap-2 text-xs text-muted-foreground opacity-60">
                <RadioGroupItem value="conical" disabled />
                <span>
                  {HEAD_THICKNESS_TEXTS.topConfig.headTypeOptions.conical}
                </span>
              </label>
            </RadioGroup>
          </div>
        </div>
      </section>

      <Separator />

      {/* Step 1: Material & Environmental Conditions */}
      <section className="space-y-4">
        <h3 className="text-base font-semibold text-foreground">
          {HEAD_THICKNESS_TEXTS.step1.title}
        </h3>

        <div className="grid grid-cols-1 items-start gap-6 md:grid-cols-2">
          {/* Left: Material Specification & Design Temperature */}
          <div className="space-y-4">
            <div className="flex flex-col gap-1.5">
              <Label className="text-xs font-normal text-muted-foreground">
                {HEAD_THICKNESS_TEXTS.step1.materialLabel}
              </Label>
              <Popover
                open={materialPopoverOpen}
                onOpenChange={setMaterialPopoverOpen}
              >
                <PopoverTrigger
                  render={
                    <Button
                      type="button"
                      variant="outline"
                      role="combobox"
                      aria-expanded={materialPopoverOpen}
                      className="h-9 w-full justify-between bg-muted/40 px-3 text-xs font-normal text-foreground"
                    />
                  }
                >
                  <span className="truncate">
                    {selectedMaterial
                      ? selectedMaterial.label
                      : HEAD_THICKNESS_TEXTS.step1.materialPlaceholder}
                  </span>
                  <ChevronsUpDown className="ml-2 size-3.5 shrink-0 opacity-50" />
                </PopoverTrigger>
                <PopoverContent className="w-80 p-0" align="start">
                  <Command>
                    <CommandInput
                      placeholder={
                        HEAD_THICKNESS_TEXTS.step1.materialSearchPlaceholder
                      }
                    />
                    <CommandList>
                      <CommandEmpty>
                        {HEAD_THICKNESS_TEXTS.step1.materialEmptyText}
                      </CommandEmpty>
                      <CommandGroup>
                        {(materialsQuery.data ?? []).map((mat) => {
                          const isSelected = mat.id === watchedValues.materialId
                          return (
                            <CommandItem
                              key={mat.id}
                              value={`${mat.id} ${mat.label}`}
                              data-checked={isSelected}
                              onSelect={() => {
                                form.setValue("materialId", mat.id, {
                                  shouldValidate: true,
                                })
                                setMaterialPopoverOpen(false)
                              }}
                            >
                              <Check
                                className={cn(
                                  "mr-1.5 size-3.5",
                                  isSelected ? "opacity-100" : "opacity-0"
                                )}
                              />
                              <span className="truncate">{mat.label}</span>
                            </CommandItem>
                          )
                        })}
                      </CommandGroup>
                    </CommandList>
                  </Command>
                </PopoverContent>
              </Popover>
            </div>

            <NumberStepperField
              id="designTemperature"
              label={HEAD_THICKNESS_TEXTS.step1.designTempLabel(
                activeUnits.temperature
              )}
              value={watchedValues.designTemperature ?? 40}
              step={5}
              error={form.formState.errors.designTemperature?.message}
              onChange={(next) =>
                form.setValue("designTemperature", next, {
                  shouldValidate: true,
                })
              }
            />
          </div>

          {/* Right: Allowable Stress (S) */}
          <div className="space-y-4">
            <h4 className="text-sm font-semibold text-foreground">
              {HEAD_THICKNESS_TEXTS.step1.allowableStressTitle}
            </h4>

            <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
              <Checkbox
                checked={Boolean(watchedValues.manualOverrideEnabled)}
                onCheckedChange={(checked) => {
                  form.setValue("manualOverrideEnabled", Boolean(checked), {
                    shouldValidate: true,
                  })
                }}
              />
              <span>{HEAD_THICKNESS_TEXTS.step1.manualOverrideLabel}</span>
            </label>

            {watchedValues.manualOverrideEnabled ? (
              <NumberStepperField
                id="allowableStressOverride"
                label={HEAD_THICKNESS_TEXTS.step1.manualInputLabel(
                  activeUnits.pressure
                )}
                value={watchedValues.allowableStressOverride ?? 138}
                step={5}
                min={0.1}
                error={form.formState.errors.allowableStressOverride?.message}
                onChange={(next) =>
                  form.setValue("allowableStressOverride", next, {
                    shouldValidate: true,
                  })
                }
              />
            ) : null}

            <MetricCard
              label={HEAD_THICKNESS_TEXTS.step1.allowableStressCardLabel}
              value={formattedAllowableStress}
            />
          </div>
        </div>
      </section>

      <Separator />

      {/* Step 2: Welding Joint & Efficiency (E) */}
      <section className="space-y-4">
        <h3 className="text-base font-semibold text-foreground">
          {HEAD_THICKNESS_TEXTS.step2.title}
        </h3>

        <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
          <div className="space-y-4">
            <div className="flex flex-col gap-1.5">
              <Label className="text-xs font-normal text-muted-foreground">
                {HEAD_THICKNESS_TEXTS.step2.jointTypeLabel}
              </Label>
              <Select
                value={watchedValues.jointTypeId}
                onValueChange={(val) => {
                  if (typeof val === "string") {
                    form.setValue("jointTypeId", val, { shouldValidate: true })
                  }
                }}
              >
                <SelectTrigger className="h-9 w-full bg-muted/40 px-3 text-xs">
                  <SelectValue>{selectedJointTypeLabel}</SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {(optionsQuery.data?.jointTypes ?? []).map((joint) => (
                    <SelectItem key={joint.id} value={joint.id}>
                      {joint.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <MetricCard
              label={HEAD_THICKNESS_TEXTS.step2.jointEfficiencyCardLabel}
              value={formattedJointEfficiency}
            />
          </div>

          <div className="space-y-4">
            <div className="flex flex-col gap-1.5">
              <Label className="text-xs font-normal text-muted-foreground">
                {HEAD_THICKNESS_TEXTS.step2.rtLabel}
              </Label>
              <Select
                value={watchedValues.rtId}
                onValueChange={(val) => {
                  if (typeof val === "string") {
                    form.setValue("rtId", val, { shouldValidate: true })
                  }
                }}
              >
                <SelectTrigger className="h-9 w-full bg-muted/40 px-3 text-xs">
                  <SelectValue>{selectedRtLabel}</SelectValue>
                </SelectTrigger>
                <SelectContent>
                  {(optionsQuery.data?.rtOptions ?? []).map((rt) => (
                    <SelectItem key={rt.id} value={rt.id}>
                      {rt.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>
      </section>

      <Separator />

      {/* Step 3: Calculation Mode & Geometry */}
      <section className="space-y-4">
        <h3 className="text-base font-semibold text-foreground">
          {HEAD_THICKNESS_TEXTS.step3.title}
        </h3>

        <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
          <div className="space-y-2">
            <Label className="text-xs font-medium text-muted-foreground">
              {HEAD_THICKNESS_TEXTS.step3.modeLabel}
            </Label>
            <RadioGroup
              value={watchedValues.mode}
              onValueChange={(val) => {
                if (val === "min_thickness") {
                  form.setValue("mode", val as CalculationMode, {
                    shouldValidate: true,
                  })
                }
              }}
              className="flex flex-col gap-2.5 pt-1"
            >
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="min_thickness" />
                <span>
                  {HEAD_THICKNESS_TEXTS.step3.modeOptions.minThickness}
                </span>
              </label>
              <label className="flex cursor-not-allowed items-center gap-2 text-xs text-muted-foreground opacity-60">
                <RadioGroupItem value="mawp" disabled />
                <span>{HEAD_THICKNESS_TEXTS.step3.modeOptions.mawp}</span>
              </label>
            </RadioGroup>
          </div>

          <div>
            <NumberStepperField
              id="corrosionAllowance"
              label={HEAD_THICKNESS_TEXTS.step3.corrosionAllowanceLabel(
                activeUnits.length
              )}
              value={watchedValues.corrosionAllowance ?? 3}
              step={0.5}
              min={0}
              error={form.formState.errors.corrosionAllowance?.message}
              onChange={(next) =>
                form.setValue("corrosionAllowance", next, {
                  shouldValidate: true,
                })
              }
            />
          </div>
        </div>
      </section>

      <Separator />

      {/* Active Logic Banner + Head Geometry Inputs + Result */}
      <section className="space-y-5">
        <div className="flex items-center justify-between">
          <Alert className="border-sky-200 bg-sky-50/80 py-3 text-sky-900 dark:border-sky-900/50 dark:bg-sky-950/30 dark:text-sky-200">
            <Pin className="size-4 text-rose-600 dark:text-rose-400" />
            <AlertDescription className="text-xs font-medium text-sky-800 dark:text-sky-200">
              {HEAD_THICKNESS_TEXTS.step4.activeLogicPrefix}{" "}
              {result?.activeLogic ?? fallbackActiveLogic}
            </AlertDescription>
          </Alert>
        </div>

        {apiErrorMessage ? (
          <Alert variant="destructive">
            <AlertTriangle className="size-4" />
            <AlertDescription>{apiErrorMessage}</AlertDescription>
          </Alert>
        ) : null}

        {result?.warnings && result.warnings.length > 0 ? (
          <div className="space-y-2">
            {result.warnings.map((warning, idx) => (
              <Alert
                key={idx}
                className="border-amber-300 bg-amber-50/80 text-amber-900 dark:border-amber-900/50 dark:bg-amber-950/30 dark:text-amber-200"
              >
                <AlertTriangle className="size-4 text-amber-700 dark:text-amber-300" />
                <AlertDescription className="text-xs text-amber-800 dark:text-amber-200">
                  {warning}
                </AlertDescription>
              </Alert>
            ))}
          </div>
        ) : null}

        <div className="grid grid-cols-1 items-start gap-6 md:grid-cols-2">
          {/* Left Column: Geometric Inputs specific to Head Type */}
          <div className="space-y-4">
            {headType === "ellipsoidal" ? (
              <NumberStepperField
                id="insideDiameter"
                label={HEAD_THICKNESS_TEXTS.step4.insideDiameterLabel(
                  activeUnits.length
                )}
                value={watchedValues.insideDiameter ?? 1200}
                step={10}
                min={0.01}
                error={form.formState.errors.insideDiameter?.message}
                onChange={(next) =>
                  form.setValue("insideDiameter", next, {
                    shouldValidate: true,
                  })
                }
              />
            ) : null}

            {headType === "torispherical" ? (
              <>
                <NumberStepperField
                  id="crownRadius"
                  label={HEAD_THICKNESS_TEXTS.step4.crownRadiusLabel(
                    activeUnits.length
                  )}
                  value={watchedValues.crownRadius ?? 1200}
                  step={10}
                  min={0.01}
                  error={form.formState.errors.crownRadius?.message}
                  onChange={(next) =>
                    form.setValue("crownRadius", next, { shouldValidate: true })
                  }
                />

                <NumberStepperField
                  id="knuckleRadius"
                  label={HEAD_THICKNESS_TEXTS.step4.knuckleRadiusLabel(
                    activeUnits.length
                  )}
                  value={watchedValues.knuckleRadius ?? 72}
                  step={5}
                  min={0.01}
                  error={form.formState.errors.knuckleRadius?.message}
                  onChange={(next) =>
                    form.setValue("knuckleRadius", next, {
                      shouldValidate: true,
                    })
                  }
                />
              </>
            ) : null}

            {headType === "hemispherical" ? (
              <NumberStepperField
                id="insideRadius"
                label={HEAD_THICKNESS_TEXTS.step4.insideRadiusLabel(
                  activeUnits.length
                )}
                value={watchedValues.insideRadius ?? 600}
                step={10}
                min={0.01}
                error={form.formState.errors.insideRadius?.message}
                onChange={(next) =>
                  form.setValue("insideRadius", next, { shouldValidate: true })
                }
              />
            ) : null}
          </div>

          {/* Right Column: Internal Design Pressure + Minimum Thickness Result + Reference Latex */}
          <div className="space-y-4">
            <NumberStepperField
              id="internalPressure"
              label={HEAD_THICKNESS_TEXTS.step4.internalPressureLabel(
                activeUnits.pressure
              )}
              value={watchedValues.internalPressure ?? 1}
              step={0.1}
              min={0.01}
              error={form.formState.errors.internalPressure?.message}
              onChange={(next) =>
                form.setValue("internalPressure", next, {
                  shouldValidate: true,
                })
              }
            />

            <div className="space-y-3 pt-1">
              <div className="flex items-center justify-between">
                <MetricCard
                  label={HEAD_THICKNESS_TEXTS.step4.minThicknessCardLabel}
                  value={formattedTotalThickness}
                />
                {calculationMutation.isPending ? (
                  <span className="text-xs text-muted-foreground">
                    {HEAD_THICKNESS_TEXTS.step4.calculatingMessage}
                  </span>
                ) : null}
              </div>

              <div className="flex flex-wrap items-center gap-2 text-xs text-muted-foreground">
                <span className="font-medium text-foreground">
                  {HEAD_THICKNESS_TEXTS.step4.referencePrefix}
                </span>
                <Latex
                  formula={result?.referenceLatex ?? fallbackReferenceLatex}
                  displayMode={false}
                  className="py-0"
                />
              </div>
            </div>
          </div>
        </div>

        {result?.breakdown ? (
          <Alert className="border-sky-200 bg-sky-50/80 py-3 text-sky-900 dark:border-sky-900/50 dark:bg-sky-950/30 dark:text-sky-200">
            <AlertDescription className="text-xs text-sky-800 dark:text-sky-200">
              {result.breakdown}
            </AlertDescription>
          </Alert>
        ) : null}
      </section>

      <Separator />

      {/* Formula Reference (ASME BPVC Section VIII Div 1 — UG-32 & Appendix 1) */}
      <section className="space-y-4">
        <h3 className="text-base font-semibold text-foreground">
          {HEAD_THICKNESS_TEXTS.formulaReference.title}
        </h3>

        <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
          {HEAD_THICKNESS_TEXTS.formulaReference.items.map((item) => (
            <div key={item.title} className="space-y-2">
              <h4 className="text-xs font-semibold text-foreground">
                {item.title}
              </h4>
              <div className="rounded-md bg-muted/20 p-3">
                <Latex formula={item.latex} displayMode />
              </div>
            </div>
          ))}
        </div>
      </section>
    </div>
  )
}
