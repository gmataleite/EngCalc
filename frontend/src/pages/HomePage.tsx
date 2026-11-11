import { Link } from "react-router-dom"
import {
  ArrowLeftRight,
  Building2,
  Calculator,
  Cog,
  Cylinder,
} from "lucide-react"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Button } from "@/components/ui/button"

export default function HomePage() {
  return (
    <div className="mx-auto max-w-6xl space-y-8">
      <div className="space-y-2">
        <h1 className="text-3xl font-bold tracking-tight">EngCalc</h1>
        <p className="text-sm text-muted-foreground">
          Modular engineering calculation suite for pressure vessels, machine
          elements, structural analysis, and unit conversions.
        </p>
      </div>

      {/* Pressure Vessels Module */}
      <section className="space-y-3">
        <h2 className="text-lg font-semibold tracking-tight">
          Pressure Vessels (ASME BPVC Section VIII Div. 1)
        </h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <Link to="/pressure-vessels/shell-thickness" className="block">
            <Card className="h-full transition-colors hover:bg-muted/30">
              <CardHeader>
                <div className="flex items-center gap-2">
                  <Calculator className="size-4 text-primary" />
                  <CardTitle>Shell Thickness (UG-27)</CardTitle>
                </div>
                <CardDescription>
                  Minimum required thickness of cylindrical shells under
                  internal pressure per ASME Section VIII Div. 1, UG-27 and
                  Mandatory Appendix 1-2.
                </CardDescription>
              </CardHeader>
            </Card>
          </Link>

          <Link to="/pressure-vessels/head-thickness" className="block">
            <Card className="h-full transition-colors hover:bg-muted/30">
              <CardHeader>
                <div className="flex items-center gap-2">
                  <Calculator className="size-4 text-primary" />
                  <CardTitle>Formed Heads (UG-32)</CardTitle>
                </div>
                <CardDescription>
                  Minimum required thickness of ellipsoidal, torispherical, and
                  hemispherical heads under internal pressure per ASME Section
                  VIII Div. 1, UG-32.
                </CardDescription>
              </CardHeader>
            </Card>
          </Link>
        </div>
      </section>

      {/* Storage Tanks module */}
      <section className="space-y-3">
        <h2 className="text-lg font-semibold tracking-tight">Storage Tanks</h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <Link to="/tanks/volume" className="block">
            <Card className="h-full transition-colors hover:bg-muted/30">
              <CardHeader>
                <div className="flex items-center gap-2">
                  <Cylinder className="size-4 text-primary" />
                  <CardTitle>Tank Volume</CardTitle>
                </div>
                <CardDescription>
                  Calculate the internal and product volume of vertical and
                  horizontal cylindrical storage tanks.
                </CardDescription>
              </CardHeader>
            </Card>
          </Link>
        </div>
      </section>

      {/* Unit Converter module */}
      <section className="space-y-3">
        <h2 className="text-lg font-semibold tracking-tight">Unit Converter</h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <Link to="/converters" className="block">
            <Card className="h-full transition-colors hover:bg-muted/30">
              <CardHeader>
                <div className="flex items-center gap-2">
                  <ArrowLeftRight className="size-4 text-primary" />
                  <CardTitle>General Unit Converter</CardTitle>
                </div>
                <CardDescription>
                  Dimensional conversion utilities across Metric (SI) and
                  Imperial engineering units (pressure, stress, torque, length,
                  thermal).
                </CardDescription>
              </CardHeader>
            </Card>
          </Link>
        </div>
      </section>

      {/* Planned Engineering Modules */}
      <section className="space-y-3">
        <h2 className="text-lg font-semibold tracking-tight">
          Upcoming Engineering Modules
        </h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <Card className="opacity-75">
            <CardHeader>
              <div className="flex items-center gap-2">
                <Cog className="size-4 text-muted-foreground" />
                <CardTitle>Shafts &amp; Machine Elements</CardTitle>
              </div>
              <CardDescription>
                Torsional and bending stress analysis, fatigue life, critical
                speed, and keyway sizing for transmission shafts.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Button
                variant="outline"
                disabled
                className="w-full justify-between"
              >
                <span>Planned Module</span>
              </Button>
            </CardContent>
          </Card>

          <Card className="opacity-75">
            <CardHeader>
              <div className="flex items-center gap-2">
                <Building2 className="size-4 text-muted-foreground" />
                <CardTitle>Structural Engineering</CardTitle>
              </div>
              <CardDescription>
                Beam deflection, section properties, column buckling, and bolted
                / welded connection checks.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Button
                variant="outline"
                disabled
                className="w-full justify-between"
              >
                <span>Planned Module</span>
              </Button>
            </CardContent>
          </Card>
        </div>
      </section>
    </div>
  )
}
