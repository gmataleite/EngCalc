import { useEffect, useRef } from "react"
import { Link, NavLink, Outlet, useLocation } from "react-router-dom"
import { ChevronDown } from "lucide-react"
import { cn } from "@/lib/utils"

export default function AppLayout() {
  const location = useLocation()
  const pressureVesselsMenuRef = useRef<HTMLDetailsElement>(null)

  useEffect(() => {
    if (pressureVesselsMenuRef.current) {
      pressureVesselsMenuRef.current.open = false
    }
  }, [location.pathname])

  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="border-b bg-card/50 px-6 py-3.5">
        <div className="mx-auto flex max-w-6xl items-center justify-between">
          <Link to="/" className="text-lg font-bold tracking-tight">
            EngCalc
          </Link>
          <nav className="flex items-center gap-6 text-sm">
            <NavLink
              to="/"
              end
              className={({ isActive }) =>
                cn(
                  "transition-colors hover:text-foreground",
                  isActive
                    ? "font-medium text-foreground"
                    : "text-muted-foreground"
                )
              }
            >
              Home
            </NavLink>
            <details ref={pressureVesselsMenuRef} className="group relative">
              <summary className="flex cursor-pointer list-none items-center gap-1 text-muted-foreground transition-colors hover:text-foreground [&::-webkit-details-marker]:hidden">
                Pressure Vessels
                <ChevronDown className="size-3.5 transition-transform group-open:rotate-180" />
              </summary>
              <div className="absolute top-full right-0 z-10 mt-2 min-w-48 rounded-md border bg-popover p-1 text-popover-foreground shadow-md">
                <NavLink
                  to="/pressure-vessels/shell-thickness"
                  className={({ isActive }) =>
                    cn(
                      "block rounded-sm px-3 py-2 text-xs transition-colors hover:bg-muted",
                      isActive
                        ? "font-medium text-foreground"
                        : "text-muted-foreground"
                    )
                  }
                >
                  Shell Thickness (UG-27)
                </NavLink>
                <NavLink
                  to="/pressure-vessels/head-thickness"
                  className={({ isActive }) =>
                    cn(
                      "block rounded-sm px-3 py-2 text-xs transition-colors hover:bg-muted",
                      isActive
                        ? "font-medium text-foreground"
                        : "text-muted-foreground"
                    )
                  }
                >
                  Formed Heads (UG-32)
                </NavLink>
              </div>
            </details>
            <NavLink
              to="/tanks/volume"
              className={({ isActive }) =>
                cn(
                  "transition-colors hover:text-foreground",
                  isActive
                    ? "font-medium text-foreground"
                    : "text-muted-foreground"
                )
              }
            >
              Tank Volume
            </NavLink>
            <NavLink
              to="/converters"
              className={({ isActive }) =>
                cn(
                  "transition-colors hover:text-foreground",
                  isActive
                    ? "font-medium text-foreground"
                    : "text-muted-foreground"
                )
              }
            >
              Unit Converter
            </NavLink>
          </nav>
        </div>
      </header>
      <main className="p-6">
        <Outlet />
      </main>
    </div>
  )
}
