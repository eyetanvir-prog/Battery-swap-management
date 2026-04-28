# Mac Setup Guide

This guide is written for someone who is not very technical and wants the easiest way to run the project on a Mac.

## Best Way To Run This Project On Mac

Use Docker.

This is the easiest option because:

- you do not need to install the .NET SDK manually
- you do not need to install SQL Server manually
- the project runs as containers from the repository

## Prerequisites

Before starting, install these:

1. [Docker Desktop for Mac](https://www.docker.com/products/docker-desktop/)
2. A web browser such as Chrome or Safari
3. Internet connection for the first run

## Download The Project

### Option 1: Download ZIP from GitHub

1. Open the GitHub repository
2. Click the green `Code` button
3. Click `Download ZIP`
4. Extract the ZIP file

### Option 2: Clone with Git

If Git is installed, run:

```bash
git clone https://github.com/eyetanvir-prog/Battery-swap-management.git
```

Then open the project folder.

## Important For M1 / M2 / M3 Macs

If the Mac uses Apple Silicon, SQL Server may need Rosetta / x86 emulation in Docker Desktop.

### Enable Rosetta support

1. Open Docker Desktop
2. Open `Settings`
3. Look for Rosetta or x86_64 emulation support
4. Turn it on
5. Restart Docker Desktop if asked

If macOS says Rosetta is missing, open Terminal and run:

```bash
softwareupdate --install-rosetta
```

## Step-By-Step To Run The Project

### 1. Open Docker Desktop

Make sure Docker Desktop is fully running before continuing.

### 2. Open Terminal

Open the `Terminal` app on the Mac.

### 3. Go to the project folder

If the project was downloaded into `Downloads`, run something like:

```bash
cd ~/Downloads/Battery-swap-management
```

If needed, type `cd ` and then drag the project folder into the Terminal window.

### 4. Start the project

Run:

```bash
docker compose up --build
```

### 5. Wait

The first run can take several minutes.

Docker will:

- download required images
- build the API container
- build the MVC container
- start SQL Server
- start the backend
- start the frontend

### 6. Open the app

When the containers are ready, open:

- MVC app: `http://localhost:8080`
- API Swagger: `http://localhost:8081`

## Login Credentials

### Admin

- email: `admin@batteryswap.local`
- password: `Admin123!`

### Employee

- username: `arif.h`
- password: `Employee123!`

## What To Test First

### Admin flow

1. Open `http://localhost:8080`
2. Login as Admin
3. Open:
   - Dashboard
   - Employees
   - Clients
   - Stations
   - Batteries
   - Map
   - Reports

### Employee flow

1. Logout
2. Login as Employee
3. Open `Swap Desk`
4. Search for a client
5. Process a swap

## How To Stop The Project

In Terminal:

1. Press `Control + C`
2. Then run:

```bash
docker compose down
```

## If Something Does Not Work

### Problem: The browser does not open the app

Try:

- wait 1 to 3 more minutes on the first run
- make sure Docker Desktop is still running
- check that `docker compose up --build` is still running in Terminal

### Problem: Port already in use

The project uses these ports:

- `8080` for MVC
- `8081` for API
- `1433` for SQL Server

If another app is already using one of these ports, Docker may fail to start.

### Problem: SQL Server container fails on Apple Silicon

Make sure:

- Docker Desktop Rosetta support is enabled
- Rosetta is installed on macOS

### Problem: Docker command not found

That usually means Docker Desktop is not installed or not started yet.

## One-Line Quick Start

If everything is installed already:

```bash
docker compose up --build
```

Then open:

- `http://localhost:8080`

## Related Docs

- Main README: `README.md`
- Demo guide: `docs/DemoGuide.md`
- Architecture: `docs/Architecture.md`
- Full project specification: `docs/ProjectSpecification.md`
