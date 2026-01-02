# Section_03_20-Database_Connection-Add_Docker_Compose_Prompt.md

## Summary

Add docker and docker compose support

## Details

Add docker and Docker Compose support to the Dot.Net project in folder "01_Code/RoyalVillaApi".

Add all the needed files to the project to manage Docker Compose for the project.

- Service "postgres"

We need a Postgres DB to our Docker Compose cluster (Docker Compose Service "postgres").

It exposes port 5432 to 5432 of the host.

Make sure we have the latest Postgres version available.

- Service "webapi"

We also want to have the Dot.Net Web API project also to Docker Compose (Docker Compose Service "webapi").

We will need a Dockerfile. make sure we pull the image of the latest Dot.Net 10 version available.

The "webapi" service depends on the "postgres" service. It will connect to the Postgres Server as user "postgres" and password "postgres123".

It exposes port to the host. Please configure these ports using the proper setup patterns.

- Dependencies between services

The "webapi" service depends on a health check provided by the "postgres" service. The check a loop that ensures Postgres is ready before "webapi" starts.

Also, a Compose network is needed for the services to communicate on.

## Documentation

Generate instructions to use and manage the Docker cluster to a separate file next to this file: "Section_03_20-Database_Connection-Add_Docker_Compose_Result.md"

Add instructions how to run the Docker cluster, stop the cluster, reset the cluster, run psql on the Docker image from the host into this file: "02_Notes/Section_03-Database_Connection.md".

## Issues to address

**Postgres container does not start:**

Looks like the container "royalvilla_postgres" does not properly start.

**WebAPI Docker Compose port on MacOS:**

MacOS ControlCenter (AirPlay Receiver) is already using port 5000. This is a common issue on macOS. Let's update the docker-compose.yml to use a different port, let's use:

```yaml
    ports:
      - "5050:8080"
      - "5051:8081"
```

Otherwise I get error when trying to browse to <http://localhost:5000/scalar/>, the browser shows:

```txt
Access to localhost was denied
You don't have authorization to view this page.

HTTP ERROR 403
```

Can you fix?
