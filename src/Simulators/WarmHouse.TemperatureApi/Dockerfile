# syntax=docker/dockerfile:1
#
# Temperature sensor simulator — partner device.
#
# The build context is the solution root, because Directory.Build.props and the
# central package versions live there and the service builds against the shared
# projects:
#
#   docker build -f src/Simulators/WarmHouse.TemperatureApi/Dockerfile -t warmhouse/temperature-api .
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
COPY src/Simulators/WarmHouse.TemperatureApi/WarmHouse.TemperatureApi.csproj src/Simulators/WarmHouse.TemperatureApi/

RUN dotnet restore "src/Simulators/WarmHouse.TemperatureApi/WarmHouse.TemperatureApi.csproj"

# Then the sources, so a code change does not invalidate the restore layer.
COPY src/Shared/ src/Shared/
COPY src/Simulators/WarmHouse.TemperatureApi/ src/Simulators/WarmHouse.TemperatureApi/

RUN dotnet publish "src/Simulators/WarmHouse.TemperatureApi/WarmHouse.TemperatureApi.csproj" \
        --configuration Release \
        --no-restore \
        --output /app/publish \
        /p:UseAppHost=false

# ------------------------------------------------------------------------------
# Runtime
# ------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS final

ENV ASPNETCORE_HTTP_PORTS=8081 \
    DOTNET_gcServer=1

# curl: used by the HEALTHCHECK below; the image ships neither curl nor wget.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8081
USER $APP_UID

HEALTHCHECK --interval=10s --timeout=5s --retries=5 --start-period=15s \
    CMD curl -fsS http://localhost:8081/health || exit 1

ENTRYPOINT ["dotnet", "WarmHouse.TemperatureApi.dll"]
