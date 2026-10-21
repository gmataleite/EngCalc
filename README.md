# Vessel-Calc

**Vessel-Calc** is a web-based SaaS (Software as a Service) application designed for mechanical and pressure vessel engineers. The platform performs rigorous pressure vessel design calculations based on the **ASME Boiler and Pressure Vessel Code (BPVC) Section VIII, Division 1**, culminating in automated, exportable engineering calculation reports.

This project combines practical mechanical engineering expertise with modern software engineering practices, applying **Domain-Driven Design (DDD)** and **Clean Architecture** to model complex normative and mathematical rules.

## 🚀 Architecture & Tech Stack

The project follows **Clean Architecture** principles, ensuring strict isolation between ASME normative domain rules, data persistence infrastructure, and the user interface.

* **Backend Language & Framework:** C# / .NET 9 (ASP.NET Core Web API)
* **Frontend:** React 19 + Vite + TypeScript + Tailwind CSS v4 + shadcn/ui + TanStack Query + React Hook Form + Zod + KaTeX
* **Database:** PostgreSQL 17 (with seeded ASME Section II Part D CSV fallback)
* **ORM:** Entity Framework Core (Npgsql)
* **Testing:** xUnit (Unit tests covering the ASME calculation engine)
* **Domain Utilities:** UnitsNet (Dimensional consistency and SI/Imperial unit conversions)
* **CI/CD:** GitHub Actions with Docker service provisioning

## ⚙️ Core Features

* **ASME Calculation Engine:**
  * Cylindrical shells under internal pressure (`UG-27` and Mandatory Appendix 1-2).
  * Formed heads (Ellipsoidal, Torispherical, and Hemispherical) under internal pressure (`UG-32` and Mandatory Appendix 1-3 / 1-4).
* **Hybrid Material Database:** Built-in standardized materials from ASME BPVC Section II Part D (Table 1A) with linear temperature interpolation and manual allowable stress override.
* **Interactive Real-Time Calculator UI:** Single-page reactive calculation workflow with debounced updates, unit system switching (Metric / Imperial), and rendered LaTeX equations.

## 🛠️ Running Locally (Development Environment)

### 1. Start the Database (Optional — Docker PostgreSQL)

From the `infra/` directory, copy `.env.example` to `.env` (if not already present) and start the PostgreSQL container:

```bash
cd infra
docker compose -f docker-compose.dev.yml up -d
```

> **Note:** If PostgreSQL is not running locally, the API automatically falls back to reading the seeded ASME materials directly from `Data/Seed/materials.csv`.

### 2. Run the Backend (.NET 9 Web API)

```bash
cd backend/VesselCalc.API
dotnet run
```

The API will start on `http://localhost:5293` and expose:
- `GET /api/materials`
- `GET /api/calculations/shell-thickness/options`
- `POST /api/calculations/shell-thickness`
- `GET /api/calculations/head-thickness/options`
- `POST /api/calculations/head-thickness`

### 3. Run the Frontend (React + Vite)

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173` in your browser and navigate to:
- **Shell Thickness (UG-27):** `/calculations/shell-thickness`
- **Formed Heads (UG-32):** `/calculations/head-thickness`
