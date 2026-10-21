# ADR 0006: Frontend Migration from Blazor WebAssembly to React (Vite + TypeScript)

**Status:** Accepted  
**Date:** 2026-10-21

## Context
Initial architectural plans envisioned a frontend built with Blazor WebAssembly (`VesselCalc.Web`). However, as the product evolved toward a highly interactive engineering SaaS requiring rich UI primitives, instant client-side form validation, debounced reactive API queries, searchable comboboxes, data tables, charts, and high-performance LaTeX mathematical formula rendering (KaTeX), the React ecosystem offered significantly stronger tooling and developer velocity.

## Decision
Replace the planned Blazor WebAssembly frontend with a Single Page Application (SPA) located in `/frontend`, built with:
* **React 19 + Vite + TypeScript** (strict mode, zero `any`)
* **Tailwind CSS v4 + shadcn/ui** for accessible, composable UI primitives
* **React Hook Form + Zod** for schema-driven input validation
* **TanStack Query + Axios** for debounced asynchronous state management against the ASP.NET Core Web API
* **KaTeX** for client-side rendering of ASME BPVC mathematical equations

Crucially, **all engineering calculation logic remains exclusively in the C# backend (`VesselCalc.Domain`)**. The React frontend only collects user inputs, validates form structure, sends HTTP requests to `VesselCalc.API`, and displays the resulting metrics, breakdown, warnings, and reference formulas.

## Consequences
* **Positive:**
  * Access to mature UI and visualization libraries (`shadcn/ui`, `TanStack Query`, `Recharts`, `KaTeX`).
  * Fast HMR and build times via Vite.
  * Clear decoupling between presentation (`/frontend`) and normative calculation logic (`/backend`), enforced by a clean REST API contract (`camelCase` JSON over HTTP).
* **Negative:**
  * DTO contracts must be maintained across C# (`VesselCalc.Application`) and TypeScript (`frontend/src/features/.../types.ts`), rather than sharing C# assemblies directly as in Blazor.
  * Requires CORS configuration in `VesselCalc.API` (`http://localhost:5173`) during local development.

