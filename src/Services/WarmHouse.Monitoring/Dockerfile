# syntax=docker/dockerfile:1
#
# Monitoring service — cameras.
#
# The build context is the solution root, because Directory.Build.props and the
# central package versions live there and the service builds against the shared
# projects:
#
#   docker build -f src/Services/WarmHouse.Monitoring/Dockerfile -t warmhouse/monitoring .
#
# Both stages use Linux-only image tags (-noble = Ubuntu 24.04); the floating
# "10.0" tags also resolve to Windows Nano Server on a Windows daemon, which
# this image must never do.

# ------------------------------------------------------------------------------
# Build
# ------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
WORKDIR /src

# Project files first: restore is cached until a dependency actually changes.
COPY Directory.Build.props Directory.Packages.props ./
COPY src/Shared/WarmHouse.Shared.Contracts/WarmHouse.Shared.Contracts.csproj src/Shared/WarmHouse.Shared.Contracts/
COPY src/Shared/WarmHouse.Shared/WarmHouse.Shared.csproj src/Shared/WarmHouse.Shared/
COPY src/Services/WarmHouse.Monitoring/WarmHouse.Monitoring.csproj src/Services/WarmHouse.Monitoring/

RUN dotnet restore "src/Services/WarmHouse.Monitoring/WarmHouse.Monitoring.csproj"

# Then the sources, so a code change does not invalidate the restore layer.
COPY src/Shared/ src/Shared/
COPY src/Services/WarmHouse.Monitoring/ src/Services/WarmHouse.Monitoring/

RUN dotnet publish "src/Services/WarmHouse.Monitoring/WarmHouse.Monitoring.csproj" \
        --configuration Release \
        --no-restore \
        --output /app/publish \
        /p:UseAppHost=false

# ------------------------------------------------------------------------------
# Runtime
# ------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_gcServer=1

# libgssapi-krb5-2: the RabbitMQ client resolves GSSAPI when opening a
#   connection and the runtime image does not ship it, so without it every
#   broker connection fails with "libgssapi_krb5.so.2: cannot open shared object".
# curl: used by the HEALTHCHECK below; the image ships neither curl nor wget.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
USER $APP_UID

HEALTHCHECK --interval=10s --timeout=5s --retries=5 --start-period=15s \
    CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "WarmHouse.Monitoring.dll"]
