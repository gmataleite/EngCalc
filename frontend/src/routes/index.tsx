import { createBrowserRouter } from "react-router-dom"
import AppLayout from "@/components/layout/AppLayout"
import HomePage from "@/pages/HomePage"
import NotFoundPage from "@/pages/NotFoundPage"
import ShellThicknessPage from "@/pages/pressure-vessels/ShellThicknessPage"
import HeadThicknessPage from "@/pages/pressure-vessels/HeadThicknessPage"
import ConverterPage from "@/pages/converters/ConverterPage"

export const router = createBrowserRouter([
  {
    path: "/",
    element: <AppLayout />,
    children: [
      { index: true, element: <HomePage /> },
      {
        path: "pressure-vessels/shell-thickness",
        element: <ShellThicknessPage />,
      },
      {
        path: "pressure-vessels/head-thickness",
        element: <HeadThicknessPage />,
      },
      {
        path: "converters",
        element: <ConverterPage />,
      },
      // Legacy / alternative paths kept for backward compatibility
      {
        path: "calculations/shell-thickness",
        element: <ShellThicknessPage />,
      },
      {
        path: "calculations/head-thickness",
        element: <HeadThicknessPage />,
      },
      {
        path: "calculations/converters",
        element: <ConverterPage />,
      },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
])
