# Docker Setup for WebApiDemo

## Overview

This project includes Docker support with a PostgreSQL database. The setup includes:

- ASP.NET Core Web API container
- PostgreSQL database container
- Network configuration for inter-container communication
- Volume persistence for database data

## Quick Start

### Prerequisites

- Docker Desktop installed and running
- Docker Compose installed (included with Docker Desktop)

### Run the Application

1. **Build and start all services:**

   ```bash
   docker-compose up --build
   ```

2. **Run in detached mode (background):**

   ```bash
   docker-compose up -d --build
   ```

3. **Stop the services:**

   ```bash
   docker-compose down
   ```

4. **Stop and remove volumes (clean reset):**

   ```bash
   docker-compose down -v
   ```

## Services

### Web API Service

- **Container Name**: `webapi_webapi_1` (auto-generated)
- **Ports**:
  - HTTP: `localhost:5000` → container port `8080`
  - HTTPS: `localhost:5001` → container port `8081`
- **API Base URL**: `http://localhost:5000/api` or `https://localhost:5001/api`

### PostgreSQL Database Service

- **Container Name**: `postgres-webapi`
- **Port**: `localhost:5432` → container port `5432`
- **Database**: `WebApiDemo`
- **Username**: `postgres`
- **Password**: `postgres123`

## Database Connection

The Web API automatically connects to the PostgreSQL database using:

```
Host=postgres;Database=WebApiDemo;Username=postgres;Password=postgres123
```

### Connect from Host Machine

You can connect to the database directly from your host machine using:

```
Host=localhost;Port=5432;Database=WebApiDemo;Username=postgres;Password=postgres123
```

## Database and Shell Access

### PostgreSQL Database Access

#### 1. **Direct psql access to your database:**

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo
```

#### 2. **psql access as postgres superuser:**

```bash
docker exec -it postgres-webapi psql -U postgres
```

#### 3. **Connect to specific database:**

```bash
docker exec -it postgres-webapi psql -U postgres -d postgres
```

### Shell Access

#### 4. **Bash shell in PostgreSQL container:**

```bash
docker exec -it postgres-webapi bash
```

#### 5. **Shell in Web API container:**

```bash
docker exec -it webapidemo-webapi-1 bash
```

*Note: Container name may vary. Use `docker ps` to see exact names.*

#### 6. **List running containers to find exact names:**

```bash
docker ps
```

### One-liner Commands

#### 7. **Run SQL commands directly:**

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "SELECT * FROM health_check;"
```

#### 8. **Check database list:**

```bash
docker exec -it postgres-webapi psql -U postgres -c "\l"
```

#### 9. **Check tables in WebApiDemo database:**

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo -c "\dt"
```

### Useful psql Commands

Once you're in the psql prompt:

```sql
\l          -- List databases
\c database -- Connect to database
\dt         -- List tables
\d table    -- Describe table structure
\q          -- Quit psql
```

### Alternative: Docker Compose exec

If you prefer using docker-compose:

```bash
# PostgreSQL access
docker-compose exec postgres psql -U postgres -d WebApiDemo

# Shell access
docker-compose exec postgres bash
docker-compose exec webapi bash
```

**Most common command:** Direct SQL access to your WebApiDemo database:

```bash
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo
```

## Development Workflow

### Initial Setup

```bash
# Clone/navigate to project directory
cd /path/to/WebAppiDemo

# Build and run
docker-compose up --build
```

### Making Changes

```bash
# After code changes, rebuild and restart
docker-compose up --build

# Or rebuild specific service
docker-compose build webapi
docker-compose up webapi
```

### Database Operations

```bash
# Access PostgreSQL container
docker exec -it postgres-webapi psql -U postgres -d WebApiDemo

# Run Entity Framework migrations (if using EF)
docker-compose exec webapi dotnet ef database update
```

### Logs

```bash
# View logs for all services
docker-compose logs

# View logs for specific service
docker-compose logs webapi
docker-compose logs postgres

# Follow logs in real-time
docker-compose logs -f webapi
```

## File Structure

```
WebAppiDemo/
├── Dockerfile              # Web API container definition
├── docker-compose.yml      # Multi-container application definition
├── .dockerignore           # Files to exclude from Docker build
├── init.sql                # PostgreSQL initialization script
└── DOCKER-README.md        # This file
```

## Configuration Details

### Environment Variables

- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:8080;https://+:8081`
- `ConnectionStrings__DefaultConnection` - Auto-configured for PostgreSQL

### Network

- **Name**: `webapi-network`
- **Type**: Bridge network for internal container communication

### Data Persistence

- **Volume**: `postgres_data`
- **Mount**: `/var/lib/postgresql/data` in PostgreSQL container
- Data persists between container restarts

## Troubleshooting

### Common Issues

1. **Port conflicts**: Ensure ports 5000, 5001, and 5432 are not in use
2. **Permission errors**: Ensure Docker has proper permissions
3. **Build fails**: Check Dockerfile and ensure all dependencies are installed

### Reset Everything

```bash
# Stop containers and remove everything
docker-compose down -v
docker system prune -f

# Rebuild from scratch
docker-compose up --build
```

### Health Check

Visit `http://localhost:5000/api` to verify the API is running.

## Production Considerations

- Change default PostgreSQL password
- Use environment variables for sensitive configuration
- Add SSL certificates for HTTPS
- Configure proper logging and monitoring
- Use multi-stage builds for smaller image sizes
- Consider using managed database services
