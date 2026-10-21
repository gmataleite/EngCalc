import { useState } from "react"
import { Minus, Plus } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"

export interface NumberStepperFieldProps {
  id: string
  label: string
  value: number
  step?: number
  min?: number
  error?: string
  onChange: (next: number) => void
}

export function NumberStepperField({
  id,
  label,
  value,
  step = 1,
  min,
  error,
  onChange,
}: NumberStepperFieldProps) {
  const [rawInput, setRawInput] = useState<string>("")
  const [isFocused, setIsFocused] = useState(false)

  const displayValue = isFocused
    ? rawInput
    : Number.isFinite(value)
      ? value.toFixed(2)
      : "0.00"

  const handleAdjust = (delta: number) => {
    const base = Number.isFinite(value) ? value : 0
    const nextRaw = Number((base + delta).toFixed(4))
    const clamped = min !== undefined ? Math.max(min, nextRaw) : nextRaw
    onChange(clamped)
    if (isFocused) {
      setRawInput(clamped.toFixed(2))
    }
  }

  return (
    <div className="flex flex-col gap-1.5">
      <Label htmlFor={id} className="text-xs font-normal text-muted-foreground">
        {label}
      </Label>
      <div className="flex h-9 items-center rounded-md border border-input bg-muted/40 px-2 focus-within:border-ring focus-within:ring-2 focus-within:ring-ring/30">
        <Input
          id={id}
          type="number"
          step="any"
          value={displayValue}
          onFocus={() => {
            setRawInput(Number.isFinite(value) ? value.toFixed(2) : "0.00")
            setIsFocused(true)
          }}
          onBlur={() => {
            setIsFocused(false)
            const parsed = Number.parseFloat(rawInput)
            if (Number.isFinite(parsed)) {
              const clamped = min !== undefined ? Math.max(min, parsed) : parsed
              onChange(clamped)
            }
          }}
          onChange={(e) => {
            const nextStr = e.target.value
            setRawInput(nextStr)
            const parsed = Number.parseFloat(nextStr)
            if (Number.isFinite(parsed)) {
              onChange(parsed)
            }
          }}
          className="h-full [appearance:textfield] border-0 bg-transparent px-1 text-sm shadow-none focus-visible:ring-0 [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
        />
        <div className="flex items-center gap-0.5 pl-1">
          <Button
            type="button"
            variant="ghost"
            size="icon-xs"
            onClick={() => handleAdjust(-step)}
            aria-label={`Decrease ${label}`}
            className="text-muted-foreground hover:text-foreground"
          >
            <Minus className="size-3.5" />
          </Button>
          <Button
            type="button"
            variant="ghost"
            size="icon-xs"
            onClick={() => handleAdjust(step)}
            aria-label={`Increase ${label}`}
            className="text-muted-foreground hover:text-foreground"
          >
            <Plus className="size-3.5" />
          </Button>
        </div>
      </div>
      {error ? <span className="text-xs text-destructive">{error}</span> : null}
    </div>
  )
}
