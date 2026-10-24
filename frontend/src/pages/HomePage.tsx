import { Link } from "react-router-dom"
import {
  ArrowRight,
  ArrowLeftRight,
  Building2,
  Calculator,
  Cog,
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
          <Card>
            <CardHeader>
              <div className="flex items-center gap-2">
                <Calculator className="size-4 text-primary" />
                <CardTitle>Shell Thickness (UG-27)</CardTitle>
              </div>
              <CardDescription>
                Minimum required thickness of cylindrical shells under internal
                pressure per ASME Section VIII Div. 1, UG-27 and Mandatory
                Appendix 1-2.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Button
                nativeButton={false}
                render={<Link to="/pressure-vessels/shell-thickness" />}
                variant="default"
                className="w-full justify-between"
              >
                <span>Open Calculator</span>
                <ArrowRight className="size-4" />
              </Button>
            </CardContent>
          </Card>

          <Card>
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
            <CardContent>
              <Button
                nativeButton={false}
                render={<Link to="/pressure-vessels/head-thickness" />}
                variant="default"
                className="w-full justify-between"
              >
                <span>Open Calculator</span>
                <ArrowRight className="size-4" />
              </Button>
            </CardContent>
          </Card>
        </div>
      </section>

      {/* Unit Converter module */}
      <section className="space-y-3">
        <h2 className="text-lg font-semibold tracking-tight">Unit Converter</h2>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <Card>
            <CardHeader>
              <div className="flex items-center gap-2">
                <ArrowLeftRight className="size-4 text-primary" />
                <CardTitle>General Unit Converter</CardTitle>
              </div>
              <CardDescription>
                Dimensional conversion utilities across Metric (SI) and Imperial
                engineering units (pressure, stress, torque, length, thermal).
              </CardDescription>
            </CardHeader>
            <CardContent>
              <Button
                nativeButton={false}
                render={<Link to="/converters" />}
                variant="default"
                className="w-full justify-between"
              >
                <span>Open Calculator</span>
                <ArrowRight className="size-4" />
              </Button>
            </CardContent>
          </Card>
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
