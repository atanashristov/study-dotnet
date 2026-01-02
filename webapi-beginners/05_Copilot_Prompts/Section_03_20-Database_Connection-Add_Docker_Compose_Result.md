# Section_03_20-Database_Connection-Add_Docker_Compose_Result.md

## Docker Compose Setup for RoyalVilla API

This document describes the Docker and Docker Compose setup added to the RoyalVilla API project.

## Files Added

### 1. `01_Code/docker-compose.yml`

Main Docker Compose configuration file that orchestrates both the Postgres database and the Web API service.

### 2. `01_Code/RoyalVillaApi/Dockerfile`

Multi-stage Dockerfile for building and running the .NET 10 Web API application.

### 3. `01_Code/RoyalVillaApi/.dockerignore`

Excludes unnecessary files from the Docker build context for faster builds.

## Architecture Overview

### Services

#### 1. postgres

- **Image**: `postgres:latest`
- **Container Name**: `royalvilla_postgres`
- **Port Mapping**: `5432:5432`
- **Credentials**:
  - Username: `postgres`
  - Password: `postgres123`
  - Database: `royalvilla`
- **Health Check**: Runs `pg_isready` every 10 seconds to ensure database is ready
- **Volume**: `postgres_data` mounted at `/var/lib/postgresql` (PostgreSQL 18+ compatible)
  - **Important**: PostgreSQL 18+ requires the volume to be mounted at `/var/lib/postgresql` instead of `/var/lib/postgresql/data` for proper data directory management and upgrade support

#### 2. webapi

- **Build**: Uses Dockerfile in `RoyalVillaApi` directory
- **Container Name**: `royalvilla_webapi`
- **Port Mapping**:
  - `5050:8080` (HTTP)
  - `5051:8081` (HTTPS if configured)
  - **Note**: Using 5050/5051 instead of 5000/5001 to avoid conflicts with macOS AirPlay Receiver
- **Dependencies**: Waits for `postgres` service to be healthy before starting
- **Connection String**: Configured via environment variable to connect to postgres service

### Network

- **Name**: `royalvilla-network`
- **Driver**: bridge
- **Purpose**: Enables communication between services

### Volumes

- **postgres_data**: Persists PostgreSQL data across container restarts

## Usage Instructions

### Prerequisites

- Docker Desktop installed and running
- Docker Compose installed (included with Docker Desktop)

### Starting the Cluster

Navigate to the `01_Code` directory and run:

```bash
cd /Users/tony.hristov/Projects/study-dotnet/webapi-beginners/01_Code
docker-compose up -d
```

The `-d` flag runs the services in detached mode (background).

### Checking Service Status

```bash
docker-compose ps
```

### Viewing Logs

View all service logs:

```bash
docker-compose logs
```

Follow logs in real-time:

```bash
docker-compose logs -f
```

View logs for a specific service:

```bash
docker-compose logs webapi
docker-compose logs postgres
```

### Stopping the Cluster

```bash
docker-compose stop
```

This stops the containers but preserves data.

### Starting Stopped Services

```bash
docker-compose start
```

### Shutting Down and Removing Containers

```bash
docker-compose down
```

To also remove volumes (WARNING: This deletes all database data):

```bash
docker-compose down -v
```

### Resetting the Cluster

To completely reset (remove containers, networks, and volumes):

```bash
docker-compose down -v
docker-compose up -d
```

### Accessing PostgreSQL

#### Using psql from the host

```bash
docker exec -it royalvilla_postgres psql -U postgres -d royalvilla
```

#### Using psql with docker-compose

```bash
docker-compose exec postgres psql -U postgres -d royalvilla
```

#### Common psql Commands

Once inside psql:

- List databases: `\l`
- Connect to database: `\c royalvilla`
- List tables: `\dt`
- Describe table: `\d tablename`
- Exit psql: `\q`

### Rebuilding the Web API

If you make changes to the code:

```bash
docker-compose up -d --build webapi
```

Or rebuild without cache:

```bash
docker-compose build --no-cache webapi
docker-compose up -d
```

### Accessing the Web API

Once running, the API is accessible at:

- HTTP: <http://localhost:5050>
- HTTPS: <https://localhost:5051> (if configured)

Scalar API Documentation:

- <http://localhost:5050/scalar/v1>

## Troubleshooting

### HTTP 403 Forbidden Error

**Issue**: Getting "Access denied" or 403 error when accessing the API.

**Causes**:
1. **Port conflict**: Port 5000 is used by macOS AirPlay Receiver - use port 5050 instead
2. **HTTPS redirection**: The app was redirecting HTTP to HTTPS in containers - fixed by conditionally disabling HTTPS redirect in Development mode

**Solution**: Access the API at <http://localhost:5050/scalar/v1> instead of port 5000.

### PostgreSQL 18+ Volume Mount Issue

**Issue**: Container exits with error about data directory format incompatibility.

**Solution**: PostgreSQL 18+ requires the volume to be mounted at `/var/lib/postgresql` instead of `/var/lib/postgresql/data`. This is already configured in the provided `docker-compose.yml`.

If you encounter this issue:
```bash
docker-compose down -v  # Remove old volumes
docker-compose up -d    # Start fresh with correct mount
```

### Service won't start

Check logs:

```bash
docker-compose logs webapi
```

### Database connection issues

Verify postgres is healthy:

```bash
docker-compose ps
```

The postgres service should show "healthy" status.

### Port conflicts

If ports 5432, 5050, or 5051 are already in use, modify the port mappings in `docker-compose.yml`.

**macOS Port 5000 Issue**: Port 5000 is used by macOS AirPlay Receiver (ControlCenter). The configuration uses ports 5050/5051 to avoid this conflict. To disable AirPlay Receiver: System Settings > General > AirDrop & Handoff > AirPlay Receiver (turn off).

### Clean slate

Remove everything and start fresh:

```bash
docker-compose down -v
docker system prune -a
docker-compose up -d
```

## Environment Variables

The following environment variables are configured in the Web API service:

- `ASPNETCORE_ENVIRONMENT`: Set to `Development`
- `ASPNETCORE_HTTP_PORTS`: Set to `8080`
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string

To modify these, edit the `docker-compose.yml` file.

## Database Connection String

The connection string used by the Web API is:

```
Host=postgres;Port=5432;Database=royalvilla;Username=postgres;Password=postgres123
```

Note: The host is `postgres` (service name) not `localhost` because containers communicate via the Docker network.
