# -------------------------
# Build React
# -------------------------
FROM node:22 AS frontend

WORKDIR /src/frontend

COPY frontend/package*.json ./
RUN npm ci

COPY frontend/ ./
RUN npm run build


# -------------------------
# Build .NET
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend

WORKDIR /src

COPY GitDashboard/*.csproj GitDashboard/
RUN dotnet restore GitDashboard/GitDashboard.csproj

COPY GitDashboard/ GitDashboard/

RUN dotnet publish GitDashboard/GitDashboard.csproj \
    -c Release \
    -o /app/publish \
    --no-restore


# -------------------------
# Final image
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=backend /app/publish ./backend
COPY --from=frontend /src/frontend/dist ./frontend

RUN apt-get update \
    && apt-get install -y nginx \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://127.0.0.1:5000

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 10000

ENTRYPOINT ["sh", "-c", "dotnet /app/backend/GitDashboard.dll & nginx -g 'daemon off;'"]