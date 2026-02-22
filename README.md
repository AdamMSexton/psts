This project uses a PostgresDB and ASP.Net web application behind an NGINX reverse proxy to provide a webhosted service to track professional hours.
The option to use adminer for DB access is included in the docker-compose.yaml, but is NOT require for proper operation.

Designed to be deployed using Docker, to install take the following steps

1. Create a PSTS folder and download this repository to that folder.  I should have "nginx" and "psts.web" folders as well as the docker-compose.yaml

2. In the psts.web folder you should have this Dockerfile to build from source:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ./psts.web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "psts.web.dll"]
```


3. Switch back to main PSTS folder and verify your docker-compose.yaml.  Recommended is below:

```YAML
services:
  nginx-proxy:
    image: nginx:latest
    container_name: psts-reverse-proxy
    restart: always
    volumes:
      - ./nginx/nginx.conf:/etc/nginx/nginx.conf:ro
    ports:
      - "80:80"
    networks:
      public_face:
      backend:
  web:
    build: ./psts.Web
    container_name: psts-web
    restart: always
    environment:
      ConnectionStrings__Default: "Host=psts-postgres;Database=psts;Username=psts_user;Password=psts_password"     #Ensure this matches your settings below
    networks:
      - backend
    depends_on:
      db:
        condition: service_healthy
  
  db:
    image: postgres:16
    container_name: psts-postgres
    restart: always
    environment:
        POSTGRES_USER: psts_user                 # Change as desired
        POSTGRES_PASSWORD: psts_password         # Change as desired
        POSTGRES_DB: psts
    volumes:
        - ./db:/var/lib/postgresql/data
    networks:
        - backend
    healthcheck:
        test: ["CMD-SHELL", "pg_isready -U postgres"]
        interval: 10s
        timeout: 5s
        retries: 5

  adminer:                                       # Optional adminer.  Makes admin access to DB available on port 8080 of PSTS IP
    image: adminer
    container_name: psts-adminer
    restart: always
    ports:
        - "8080:8080"
    networks:
      - backend

networks:
  backend:
    driver: bridge
  public_face:
    driver: bridge
```
You can specify an IP address, but ensure you use the nginx-proxy container as the destination.

4. Run the following commands from the PSTS folder:
````Bash
docker compose build
docker compose up -d
````
5. Navigate to ASSIGNED-IP:80 for access
