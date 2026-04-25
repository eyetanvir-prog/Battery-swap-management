# Battery Swap Station Management System

University Project - Full Documentation (Plain English + Technical + Architectural)

Stack: ASP.NET Core 10 Web API, ASP.NET Core 10 MVC Frontend, SQL Server 2022, Docker Compose, Docker Hub

## Snapshot

- User roles: 3
- System modules: 8
- Database tables: 7
- Cost per swap: 140 Tk

## 1. What Is This System?

This system manages battery swap stations for electric vehicles such as rickshaws and bikes. A driver brings in a depleted battery, exchanges it for a charged one, and pays 140 Tk per swap.

### User Roles

#### Admin

- Controls the entire system
- Manages employees, clients, stations, and batteries
- Views system-wide dashboard and reports
- Adds balance to any client account

#### Employee

- Assigned to exactly one station
- Registers new clients and captures photos
- Adds balance to client accounts
- Processes battery swaps
- Views own station stats only

#### Client

- EV driver
- Registered by Admin or Employee
- Starts with 1,000 Tk balance on registration
- Swaps battery for 140 Tk each time
- Can top up balance at any station

## 2. System Modules

1. Admin Management
2. Employee Management
3. Client Management
4. Station Management
5. Battery Management
6. Wallet / Balance
7. Battery Swap
8. Dashboard and Map

## 3. Business Workflows

### Client Registration Workflow

1. Open client registration form
2. Capture client photo
3. Fill in identity and vehicle information
4. Receive 1,000 Tk registration payment
5. Save client with starting balance of 1,000 Tk

### Battery Swap Workflow

1. Search client by phone or client ID
2. Load profile, current battery, and balance
3. Verify balance is at least 140 Tk
4. Verify current station has at least one available battery
5. Receive old battery physically
6. Select the new available battery
7. Deduct 140 Tk atomically
8. Mark returned battery as available and assigned battery as with client
9. Update client profile with new battery
10. Save swap transaction and refresh station stock

### Balance Recharge Workflow

1. Search client by phone
2. Enter recharge amount
3. Add balance and store recharge history

## 4. Technical Stack

### Backend

- ASP.NET Core 10 Web API
- C# 13
- Entity Framework Core 10
- SQL Server 2022
- JWT Bearer authentication
- Swagger / OpenAPI
- FluentValidation
- Serilog
- BCrypt password hashing

### Frontend

- ASP.NET Core 10 MVC
- Razor Views
- HttpClientFactory
- Bootstrap 5
- Leaflet.js
- Chart.js
- MediaDevices API for photo capture
- Cookie/session-based auth storage for JWT forwarding

## 5. Architecture

Three-tier architecture:

- Presentation: MVC frontend
- Application: Web API backend
- Data: SQL Server

### Containers

- `batteryswap-mvc`
- `batteryswap-api`
- `batteryswap-db`
- shared photo volume

### Request Flow

Browser -> MVC Controller -> HttpClient -> Web API -> EF Core -> SQL Server

## 6. Database Schema

The current planned schema includes these tables:

1. `admins`
2. `employees`
3. `clients`
4. `stations`
5. `batteries`
6. `swaps`
7. `balance_recharges`

## 7. API Surface

### Authentication

- `POST /api/auth/login`

### Employees

- `GET /api/employees`
- `GET /api/employees/{id}`
- `POST /api/employees`
- `PUT /api/employees/{id}`
- `DELETE /api/employees/{id}`

### Clients

- `GET /api/clients`
- `GET /api/clients/{id}`
- `GET /api/clients/search?phone={phone}`
- `POST /api/clients`
- `PUT /api/clients/{id}`
- `POST /api/clients/{id}/recharge`

### Stations

- `GET /api/stations`
- `GET /api/stations/{id}`
- `POST /api/stations`
- `PUT /api/stations/{id}`

### Batteries

- `GET /api/batteries`
- `GET /api/batteries/station/{stationId}`
- `GET /api/batteries/available/{stationId}`
- `POST /api/batteries`

### Swaps

- `POST /api/swaps`
- `GET /api/swaps`
- `GET /api/swaps/client/{clientId}`
- `GET /api/swaps/station/{stationId}`

### Dashboard

- `GET /api/dashboard/admin`
- `GET /api/dashboard/employee/{stationId}`

## 8. Solution Structure

```text
BatterySwap.slnx
BatterySwap.API/
BatterySwap.MVC/
BatterySwap.Shared/
docs/
```

## 9. Suggested Implementation Order

1. Foundation
2. Authentication
3. CRUD
4. Core business logic
5. Dashboard
6. Map
7. Polish and deployment

## Notes

- The original brief mentioned 6 database tables, but the defined schema lists 7.
- The first implementation slice in this workspace starts with Phase 1 foundation work.
