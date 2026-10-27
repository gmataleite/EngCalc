import { useEffect, useMemo, useState } from "react"
import { useForm, useWatch } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useMutation, useQuery } from "@tanstack/react-query"
import { AxiosError } from "axios"
import { Check, ChevronsUpDown, AlertTriangle } from "lucide-react"

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
import { useDebounce } from "@/hooks/useDebounce"
import { cn } from "@/lib/utils"

import {
  calculateShellThickness,
  fetchMaterials,
  fetchShellThicknessOptions,
} from "./api"
import { SHELL_THICKNESS_TEXTS } from "./constants"
import {
  DEFAULT_SHELL_THICKNESS_VALUES,
  shellThicknessFormSchema,
  type ShellThicknessFormValues,
} from "./schema"
import type {
  CalculationMode,
  ShellThicknessCalculationRequest,
  ShellType,
  UnitSystem,
} from "./types"

export default function ShellThicknessPage() {
  const [materialPopoverOpen, setMaterialPopoverOpen] = useState(false)

  const form = useForm<ShellThicknessFormValues>({
    resolver: zodResolver(shellThicknessFormSchema),
    defaultValues: DEFAULT_SHELL_THICKNESS_VALUES,
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
    queryKey: ["shell-thickness-options"],
    queryFn: fetchShellThicknessOptions,
  })

  const calculationMutation = useMutation({
    mutationFn: calculateShellThickness,
  })

  const unitSystem: UnitSystem = watchedValues.unitSystem ?? "metric"
  const mode: CalculationMode = watchedValues.mode ?? "min_thickness"
  const activeUnits = SHELL_THICKNESS_TEXTS.units[unitSystem]

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
  const resetCalculation = calculationMutation.reset
  useEffect(() => {
    const parsed = shellThicknessFormSchema.safeParse(debouncedValues)
    if (!parsed.success) {
      resetCalculation()
      return
    }

    const v = parsed.data
    const payload: ShellThicknessCalculationRequest = {
      unitSystem: v.unitSystem,
      shellType: v.shellType,
      mode: v.mode,
      materialId: v.materialId,
      designTemperature: v.designTemperature,
      insideRadius: v.insideRadius,
      internalPressure:
        v.mode === "min_thickness" ? v.internalPressure : undefined,
      thickness: v.mode === "mawp" ? v.thickness : undefined,
      jointTypeId: v.jointTypeId,
      rtId: v.rtId,
      corrosionAllowance: v.corrosionAllowance,
      allowableStressOverride: v.manualOverrideEnabled
        ? v.allowableStressOverride
        : undefined,
    }

    mutateCalculation(payload)
  }, [debouncedValues, mutateCalculation, resetCalculation])

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
        | { message?: string }
        | undefined
      return (
        responseData?.message ??
        calculationMutation.error.message ??
        "Unable to compute shell calculation."
      )
    }
    return calculationMutation.error instanceof Error
      ? calculationMutation.error.message
      : "Unable to compute shell calculation."
  }, [calculationMutation.error])

  const formattedAllowableStress = result
    ? `${result.allowableStress.toFixed(2)} ${result.pressureUnit}`
    : `— ${activeUnits.pressure}`

  const formattedJointEfficiency = result
    ? `${result.jointEfficiencyPercent.toFixed(0)} %`
    : "— %"

  const formattedTotalThickness = result
    ? `${result.totalMinimumThickness.toFixed(4)} ${result.lengthUnit}`
    : `— ${activeUnits.length}`

  const formattedMawp =
    result?.governingMawp !== undefined && result?.governingMawp !== null
      ? `${result.governingMawp.toFixed(4)} ${result.pressureUnit}`
      : `— ${activeUnits.pressure}`

  return (
    <div className="mx-auto max-w-6xl space-y-6 pb-12">
      {/* Page Title & Section 1: Configuration Radio Groups */}
      <section className="space-y-4">
        <div className="space-y-1">
          <h1 className="text-2xl font-bold tracking-tight text-foreground sm:text-3xl">
            {SHELL_THICKNESS_TEXTS.pageTitle}
          </h1>
          <h2 className="text-base font-semibold text-foreground/90 sm:text-lg">
            {SHELL_THICKNESS_TEXTS.pageSubtitle}
          </h2>
        </div>

        <div className="grid grid-cols-1 gap-6 pt-2 md:grid-cols-3">
          {/* Unit System */}
          <div className="space-y-2">
            <Label className="text-xs font-medium text-muted-foreground">
              {SHELL_THICKNESS_TEXTS.section1.unitSystemLabel}
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
                  {SHELL_THICKNESS_TEXTS.section1.unitOptions.imperial}
                </span>
              </label>
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="metric" />
                <span>{SHELL_THICKNESS_TEXTS.section1.unitOptions.metric}</span>
              </label>
            </RadioGroup>
          </div>

          {/* Shell Type */}
          <div className="space-y-2">
            <Label className="text-xs font-medium text-muted-foreground">
              {SHELL_THICKNESS_TEXTS.section1.shellTypeLabel}
            </Label>
            <RadioGroup
              value={watchedValues.shellType}
              onValueChange={(val) => {
                if (val === "cylindrical") {
                  form.setValue("shellType", val as ShellType, {
                    shouldValidate: true,
                  })
                }
              }}
              className="flex flex-wrap items-center gap-4 pt-1"
            >
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="cylindrical" />
                <span>
                  {SHELL_THICKNESS_TEXTS.section1.shellTypeOptions.cylindrical}
                </span>
              </label>
              <label className="flex cursor-not-allowed items-center gap-2 text-xs text-muted-foreground opacity-60">
                <RadioGroupItem value="spherical" disabled />
                <span>
                  {SHELL_THICKNESS_TEXTS.section1.shellTypeOptions.spherical}
                </span>
              </label>
            </RadioGroup>
          </div>

          {/* Calculation Mode */}
          <div className="space-y-2">
            <Label className="text-xs font-medium text-muted-foreground">
              {SHELL_THICKNESS_TEXTS.section1.modeLabel}
            </Label>
            <RadioGroup
              value={watchedValues.mode}
              onValueChange={(val) => {
                if (val === "min_thickness" || val === "mawp") {
                  form.setValue("mode", val as CalculationMode, {
                    shouldValidate: true,
                  })
                  void form.trigger([
                    "thickness",
                    "corrosionAllowance",
                    "internalPressure",
                  ])
                }
              }}
              className="flex flex-wrap items-center gap-4 pt-1"
            >
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="min_thickness" />
                <span>
                  {SHELL_THICKNESS_TEXTS.section1.modeOptions.minThickness}
                </span>
              </label>
              <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
                <RadioGroupItem value="mawp" />
                <span>{SHELL_THICKNESS_TEXTS.section1.modeOptions.mawp}</span>
              </label>
            </RadioGroup>
          </div>
        </div>
      </section>

      <Separator />

      {/* Section 2: Material & Environment | Geometry & Target | Fabrication & Corrosion */}
      <section className="grid grid-cols-1 gap-6 md:grid-cols-3">
        {/* Column 1: Material & Environment */}
        <div className="space-y-4">
          <h3 className="text-sm font-semibold text-foreground">
            {SHELL_THICKNESS_TEXTS.section2.materialColumnTitle}
          </h3>

          <div className="flex flex-col gap-1.5">
            <Label className="text-xs font-normal text-muted-foreground">
              {SHELL_THICKNESS_TEXTS.section2.materialLabel}
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
                    : SHELL_THICKNESS_TEXTS.section2.materialPlaceholder}
                </span>
                <ChevronsUpDown className="ml-2 size-3.5 shrink-0 opacity-50" />
              </PopoverTrigger>
              <PopoverContent className="w-80 p-0" align="start">
                <Command>
                  <CommandInput
                    placeholder={
                      SHELL_THICKNESS_TEXTS.section2.materialSearchPlaceholder
                    }
                  />
                  <CommandList>
                    <CommandEmpty>
                      {SHELL_THICKNESS_TEXTS.section2.materialEmptyText}
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
            label={SHELL_THICKNESS_TEXTS.section2.designTempLabel(
              activeUnits.temperature
            )}
            value={watchedValues.designTemperature ?? 40}
            step={5}
            error={form.formState.errors.designTemperature?.message}
            onChange={(next) =>
              form.setValue("designTemperature", next, { shouldValidate: true })
            }
          />
        </div>

        {/* Column 2: Geometry & Target */}
        <div className="space-y-4">
          <h3 className="text-sm font-semibold text-foreground">
            {SHELL_THICKNESS_TEXTS.section2.geometryColumnTitle}
          </h3>

          <NumberStepperField
            id="insideRadius"
            label={SHELL_THICKNESS_TEXTS.section2.insideRadiusLabel(
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

          {mode === "mawp" ? (
            <NumberStepperField
              id="thickness"
              label={SHELL_THICKNESS_TEXTS.section2.nominalThicknessLabel(
                activeUnits.length
              )}
              value={watchedValues.thickness ?? 12}
              step={0.5}
              min={0.01}
              error={form.formState.errors.thickness?.message}
              onChange={(next) => {
                form.setValue("thickness", next, { shouldValidate: true })
                void form.trigger("corrosionAllowance")
              }}
            />
          ) : (
            <NumberStepperField
              id="internalPressure"
              label={SHELL_THICKNESS_TEXTS.section2.internalPressureLabel(
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
          )}
        </div>

        {/* Column 3: Fabrication & Corrosion */}
        <div className="space-y-4">
          <h3 className="text-sm font-semibold text-foreground">
            {SHELL_THICKNESS_TEXTS.section2.fabricationColumnTitle}
          </h3>

          <div className="flex flex-col gap-1.5">
            <Label className="text-xs font-normal text-muted-foreground">
              {SHELL_THICKNESS_TEXTS.section2.jointTypeLabel}
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

          <div className="flex flex-col gap-1.5">
            <Label className="text-xs font-normal text-muted-foreground">
              {SHELL_THICKNESS_TEXTS.section2.rtLabel}
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

          <NumberStepperField
            id="corrosionAllowance"
            label={SHELL_THICKNESS_TEXTS.section2.corrosionAllowanceLabel(
              activeUnits.length
            )}
            value={watchedValues.corrosionAllowance ?? 3}
            step={0.5}
            min={0}
            error={form.formState.errors.corrosionAllowance?.message}
            onChange={(next) => {
              form.setValue("corrosionAllowance", next, {
                shouldValidate: true,
              })
              void form.trigger("thickness")
            }}
          />
        </div>
      </section>

      <Separator />

      {/* Section 3: Allowable Stress (S) Configuration */}
      <section className="space-y-4">
        <h3 className="text-sm font-semibold text-foreground">
          {SHELL_THICKNESS_TEXTS.section3.title}
        </h3>

        <div className="max-w-xl space-y-3">
          <label className="flex cursor-pointer items-center gap-2 text-xs text-foreground">
            <Checkbox
              checked={Boolean(watchedValues.manualOverrideEnabled)}
              onCheckedChange={(checked) => {
                form.setValue("manualOverrideEnabled", Boolean(checked), {
                  shouldValidate: true,
                })
                void form.trigger("allowableStressOverride")
              }}
            />
            <span>{SHELL_THICKNESS_TEXTS.section3.manualOverrideLabel}</span>
          </label>

          {watchedValues.manualOverrideEnabled ? (
            <NumberStepperField
              id="allowableStressOverride"
              label={SHELL_THICKNESS_TEXTS.section3.manualInputLabel(
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
          ) : (
            <Alert className="border-sky-200 bg-sky-50/80 py-3 text-sky-900 dark:border-sky-900/50 dark:bg-sky-950/30 dark:text-sky-200">
              <AlertDescription className="text-xs text-sky-800 dark:text-sky-200">
                {SHELL_THICKNESS_TEXTS.section3.autoInterpolatedMessage}
              </AlertDescription>
            </Alert>
          )}
        </div>
      </section>

      <Separator />

      {/* Section 4: Calculation Results */}
      <section className="space-y-4">
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-semibold text-foreground">
            {SHELL_THICKNESS_TEXTS.section4.title}
          </h3>
          {calculationMutation.isPending ? (
            <span className="text-xs text-muted-foreground">
              {SHELL_THICKNESS_TEXTS.section4.calculatingMessage}
            </span>
          ) : null}
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

        <div className="grid grid-cols-1 gap-6 sm:grid-cols-3">
          <MetricCard
            label={SHELL_THICKNESS_TEXTS.section4.jointEfficiencyCardLabel}
            value={formattedJointEfficiency}
          />
          <MetricCard
            label={SHELL_THICKNESS_TEXTS.section4.allowableStressCardLabel}
            value={formattedAllowableStress}
          />
          {mode === "mawp" ? (
            <MetricCard
              label={SHELL_THICKNESS_TEXTS.section4.mawpCardLabel}
              value={formattedMawp}
            />
          ) : (
            <MetricCard
              label={SHELL_THICKNESS_TEXTS.section4.totalThicknessCardLabel}
              value={formattedTotalThickness}
            />
          )}
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

      {/* Section 5: Formula Reference (ASME BPVC Section VIII Div 1) */}
      <section className="space-y-4">
        <h3 className="text-sm font-semibold text-foreground">
          {SHELL_THICKNESS_TEXTS.section5.title}
        </h3>

        <div className="grid grid-cols-1 gap-6 md:grid-cols-2">
          {SHELL_THICKNESS_TEXTS.section5.items.map((item) => (
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
