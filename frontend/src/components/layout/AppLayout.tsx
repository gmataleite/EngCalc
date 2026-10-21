import { Outlet } from "react-router-dom"

export default function AppLayout() {
  return (
    <div className="min-h-screen">
      <header className="border-b p-4 font-semibold">VesselCalc</header>
      <main className="p-6">
        <Outlet />
      </main>
    </div>
  )
}