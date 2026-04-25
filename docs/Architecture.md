# Architecture Overview

## Three-Tier Structure

- `BatterySwap.MVC` is the presentation layer.
- `BatterySwap.API` is the application and business logic layer.
- `SQL Server` is the persistence layer.

The MVC app does not talk to the database directly. All business operations pass through the API.

## Request Flow

```mermaid
flowchart LR
    A["Browser"] --> B["BatterySwap.MVC<br/>Razor + AdminLTE"]
    B --> C["HttpClientFactory<br/>JWT Forwarding"]
    C --> D["BatterySwap.API<br/>Controllers + Services"]
    D --> E["EF Core 10"]
    E --> F["SQL Server 2022"]
```

## Main Runtime Components

- `AccountController` signs users in through the API and stores JWT/session state.
- `BatterySwapApiService` acts as the MVC-side gateway for API communication.
- `SwapService` handles atomic battery swap processing.
- `WalletService` handles balance recharge transactions.
- `AppInitializer` applies migrations and seeds demo data on startup.

## Security Model

- API authentication uses `JWT Bearer`.
- MVC keeps the JWT inside server-side session state.
- Role checks are enforced both in the API and in the MVC navigation/workflows.
- Employee users are restricted to their own station context for swap and operational views.

## Deployment Shape

```mermaid
flowchart TD
    subgraph Docker
        MVC["batteryswap-mvc<br/>Port 8080"]
        API["batteryswap-api<br/>Port 8081"]
        DB["batteryswap-db<br/>Port 1433"]
    end

    MVC --> API
    API --> DB
```

## Demo Notes

- On a fresh database, startup seeding creates a realistic demo state.
- Swagger is exposed from the API root for quick backend inspection.
- MVC exposes `/health` and API exposes `/health` for environment checks and container health probes.
