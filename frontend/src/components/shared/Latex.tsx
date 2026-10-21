import { useMemo } from "react"
import katex from "katex"
import "katex/dist/katex.min.css"
import { cn } from "@/lib/utils"

interface LatexProps {
  formula: string
  displayMode?: boolean
  className?: string
}

export function Latex({ formula, displayMode = true, className }: LatexProps) {
  const html = useMemo(() => {
    return katex.renderToString(formula, {
      displayMode,
      throwOnError: false,
      strict: false,
      trust: false,
    })
  }, [formula, displayMode])

  return (
    <div
      className={cn("overflow-x-auto py-1 text-foreground", className)}
      dangerouslySetInnerHTML={{ __html: html }}
    />
  )
}
