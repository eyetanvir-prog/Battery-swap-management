# Battery Swap Demo Guide

This guide is meant for presentation day, viva, or supervisor review.

## Demo Credentials

- Admin: `admin@batteryswap.local` / `Admin123!`
- Employee: `arif.h` / `Employee123!`
- Employee: `jerin.s` / `Employee123!`
- Employee: `farhan.a` / `Employee123!`

## Main Demo URLs

- MVC app: `http://localhost:8080`
- API Swagger: `http://localhost:8081`

## Best Demo Flow

### 1. Login as Admin

Show:

- compact AdminLTE dashboard
- live KPI cards
- battery availability chart
- recent activity
- station stock table

Mention:

- data is loaded from the API, not hardcoded in the MVC frontend
- SQL Server is the single source of truth

### 2. Open Employees

Show:

- employee list with pagination and export buttons
- create, edit, and deactivate actions
- one employee assigned to one station

### 3. Open Clients

Show:

- client list with balance and battery info
- client profile page
- recharge action
- edit/deactivate restrictions

Good talking point:

- a client holding a battery cannot be deactivated

### 4. Open Stations and Batteries

Show:

- station list and station map
- battery list with status tracking
- battery edit restrictions when a battery is currently with a client

### 5. Login as Employee

Use:

- `arif.h` / `Employee123!`

Show:

- employee-specific dashboard
- swap desk
- client search by phone
- available station batteries

### 6. Process a Swap

Show:

- client lookup
- balance validation
- available battery selection
- successful swap confirmation

Good talking point:

- swap cost is fixed at `140 Tk`
- the operation is handled atomically in the API

### 7. Open History and Reports

Show:

- swap history
- recharge history
- printable report summary

Good talking point:

- reports can be printed directly for supervisor review

## Suggested Talking Points

- The solution is split into `MVC`, `Web API`, and `SQL Server`.
- The MVC app never reads the database directly.
- Authentication uses JWT on the API side and session storage on the MVC side.
- Employee actions are station-aware and role-restricted.
- The project supports both operational use and presentation/reporting use.

## Notes About Seed Data

On a fresh database, startup seeding creates:

- 4 demo stations
- 3 demo employees
- 4 demo clients
- multiple batteries per station
- recharge history
- swap history

If the database already contains real/demo activity, seeding is designed to avoid duplicating the core records unnecessarily.
