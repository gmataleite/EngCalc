# EngCalc

**EngCalc** is a modular web-based SaaS (Software as a Service) platform designed for mechanical and structural engineers. Originally started as `Vessel-Calc` for pressure vessel design, the platform is architected to cover multiple engineering domains — including **Pressure Vessels (ASME BPVC Section VIII, Division 1)**, **Shafts & Machine Elements**, **Structural Analysis**, and **Engineering Unit Conversions** — culminating in automated, exportable engineering calculation reports.

This project combines practical engineering expertise with modern software engineering practices, applying **Domain-Driven Design (DDD)** and **Clean Architecture** to model complex normative and mathematical rules in isolated, extensible domain modules.

## 🚀 Architecture & Tech Stack

The project follows **Clean Architecture** principles with domain-based modularization, ensuring strict isolation between normative engineering domain rules, data persistence infrastructure, and the user interface.

* **Backend Language & Framework:** C# / .NET 9 (ASP.NET Core Web API)
* **Frontend:** React 19 + Vite + TypeScript + Tailwind CSS v4 + shadcn/ui + TanStack Query + React Hook Form + Zod + KaTeX
* **Database:** PostgreSQL 17 (with seeded ASME Section II Part D CSV fallback)
* **ORM:** Entity Framework Core (Npgsql)
* **Testing:** xUnit (Unit tests covering the calculation engine)
* **Domain Utilities:** UnitsNet (Dimensional consistency and SI/Imperial unit conversions)
* **CI/CD:** GitHub Actions with automated formatting, linting, and testing

## ⚙️ Engineering Modules

* **Pressure Vessels (`PressureVessels`):**
  * Cylindrical shells under internal pressure (`UG-27` and Mandatory Appendix 1-2).
  * Formed heads (Ellipsoidal, Torispherical, and Hemispherical) under internal pressure (`UG-32` and Mandatory Appendix 1-3 / 1-4).
* **Shared Material Database (`Materials`):** Built-in standardized materials from ASME BPVC Section II Part D (Table 1A) with linear temperature interpolation and manual allowable stress override.
* **Planned Modules:**
  * **Shafts & Machine Elements (`Shafts`):** Torsional/bending stress, fatigue, critical speed, keyways.
  * **Structures (`Structures`):** Beam deflection, section properties, column buckling, connections.
  * **Unit Conversions (`UnitConversions`):** Interactive engineering unit converter.

## 🛠️ Running Locally (Development Environment)

### 1. Start the Database (Optional — Docker PostgreSQL)

From the `infra/` directory, copy `.env.example` to `.env` (if not already present) and start the PostgreSQL container:

```bash
cd infra
docker compose -f docker-compose.dev.yml up -d
```

> **Note:** If PostgreSQL is not running locally, the API automatically falls back to reading the seeded materials directly from `Data/Seed/materials.csv`.

### 2. Run the Backend (.NET 9 Web API)

```bash
cd backend/EngCalc.API
dotnet run
```

The API will start on `http://localhost:5293` and expose:
- `GET /api/materials`
- `GET /api/calculations/pressure-vessels/shell-thickness/options`
- `POST /api/calculations/pressure-vessels/shell-thickness`
- `GET /api/calculations/pressure-vessels/head-thickness/options`
- `POST /api/calculations/pressure-vessels/head-thickness`

### 3. Run the Frontend (React + Vite)

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173` in your browser and navigate to:
- **Shell Thickness (UG-27):** `/pressure-vessels/shell-thickness`
- **Formed Heads (UG-32):** `/pressure-vessels/head-thickness`
