import { Link } from "react-router-dom"
import { ArrowRight, Calculator } from "lucide-react"
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
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="space-y-2">
        <h1 className="text-3xl font-bold tracking-tight">VesselCalc</h1>
        <p className="text-sm text-muted-foreground">
          Pressure vessel mechanical design calculations according to ASME BPVC
          Section VIII Division 1.
        </p>
      </div>

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
              render={<Link to="/calculations/shell-thickness" />}
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
              hemispherical heads under internal pressure per ASME Section VIII
              Div. 1, UG-32.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button
              nativeButton={false}
              render={<Link to="/calculations/head-thickness" />}
              variant="default"
              className="w-full justify-between"
            >
              <span>Open Calculator</span>
              <ArrowRight className="size-4" />
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}
