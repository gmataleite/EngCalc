import { Link, NavLink, Outlet } from "react-router-dom"
import { cn } from "@/lib/utils"

export default function AppLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="border-b bg-card/50 px-6 py-3.5">
        <div className="mx-auto flex max-w-6xl items-center justify-between">
          <Link to="/" className="text-lg font-bold tracking-tight">
            VesselCalc
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
            <NavLink
              to="/calculations/shell-thickness"
              className={({ isActive }) =>
                cn(
                  "transition-colors hover:text-foreground",
                  isActive
                    ? "font-medium text-foreground"
                    : "text-muted-foreground"
                )
              }
            >
              Shell Thickness (UG-27)
            </NavLink>
            <NavLink
              to="/calculations/head-thickness"
              className={({ isActive }) =>
                cn(
                  "transition-colors hover:text-foreground",
                  isActive
                    ? "font-medium text-foreground"
                    : "text-muted-foreground"
                )
              }
            >
              Formed Heads (UG-32)
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
