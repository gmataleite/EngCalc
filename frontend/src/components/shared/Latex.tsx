import { useEffect, useRef } from "react"
import katex from "katex"
import "katex/dist/katex.min.css"
import { cn } from "@/lib/utils"

interface LatexProps {
  formula: string
  displayMode?: boolean
  className?: string
}

export function Latex({ formula, displayMode = true, className }: LatexProps) {
  const containerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!containerRef.current) return

    katex.render(formula, containerRef.current, {
      displayMode,
      throwOnError: false,
      strict: false,
      trust: false,
    })
  }, [formula, displayMode])

  return (
    <div
      ref={containerRef}
      className={cn(
        "overflow-x-auto py-1 text-xs text-foreground [&_.katex]:text-[0.9em] sm:[&_.katex]:text-[0.95em] [&_.katex-display]:my-0.5",
        className
      )}
    />
  )
}
