import { createBrowserRouter } from "react-router-dom"
import AppLayout from "@/components/layout/AppLayout"
import HomePage from "@/pages/HomePage"
import NotFoundPage from "@/pages/NotFoundPage"
import ShellThicknessPage from "@/pages/ShellThicknessPage"
import HeadThicknessPage from "@/pages/HeadThicknessPage"

export const router = createBrowserRouter([
  {
    path: "/",
    element: <AppLayout />,
    children: [
      { index: true, element: <HomePage /> },
      {
        path: "calculations/shell-thickness",
        element: <ShellThicknessPage />,
      },
      {
        path: "calculations/head-thickness",
        element: <HeadThicknessPage />,
      },
      { path: "*", element: <NotFoundPage /> },
    ],
  },
])
