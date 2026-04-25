# Battery Swap Station Management System

University project built with ASP.NET Core 10 Web API, ASP.NET Core 10 MVC, SQL Server 2022, and Docker Compose.

## Projects

- `BatterySwap.API`: backend REST API, EF Core data access, authentication, business logic.
- `BatterySwap.MVC`: frontend MVC application that talks to the API over HTTP.
- `BatterySwap.Shared`: shared constants and cross-project types.

## Current Status

The project is now implemented as a working end-to-end demo system with:

- JWT-based API authentication for `Admin` and `Employee`
- AdminLTE-based MVC frontend with compact DataTables tables
- employee, client, station, battery, recharge, swap, history, map, dashboard, and reports modules
- live SQL Server integration through EF Core
- Swagger exposed from the API root
- seeded demo data for stations, employees, clients, batteries, recharges, and swaps on fresh databases
- printable reporting and branded login/error/session UX

## Planned Phases

1. Foundation
2. Authentication
3. CRUD for employees, clients, stations, batteries
4. Swap workflow and balance recharge
5. Dashboard
6. Map
7. Polish and deployment

## Demo Credentials

- Admin: `admin@batteryswap.local` / `Admin123!`
- Employee examples: `arif.h`, `jerin.s`, `farhan.a` / `Employee123!`

## Local Run

### API

```powershell
dotnet run --project .\BatterySwap.API
```

### MVC

```powershell
dotnet run --project .\BatterySwap.MVC
```

### Docker Compose

```powershell
docker compose up --build
```

### Demo Helper Scripts

```powershell
.\scripts\Start-Demo.ps1
.\scripts\Stop-Demo.ps1
```

## Documentation

- Main specification: `docs/ProjectSpecification.md`
- Demo walkthrough: `docs/DemoGuide.md`
- Architecture overview: `docs/Architecture.md`
