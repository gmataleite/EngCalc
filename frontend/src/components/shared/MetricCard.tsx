import { cn } from "@/lib/utils"

interface MetricCardProps {
  label: string
  value: string
  subtext?: string
  className?: string
}

export function MetricCard({
  label,
  value,
  subtext,
  className,
}: MetricCardProps) {
  return (
    <div className={cn("flex flex-col gap-1", className)}>
      <span className="text-xs font-medium text-muted-foreground">{label}</span>
      <span className="text-base font-semibold text-foreground/90 sm:text-lg">
        {value}
      </span>
      {subtext ? (
        <span className="text-xs text-muted-foreground">{subtext}</span>
      ) : null}
    </div>
  )
}
