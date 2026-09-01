# One multi-stage Dockerfile for every service. The build context is the
# solution root so that Directory.Build.props and the central package versions
# apply, and each image differs only by its build arguments.
# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG PROJECT_PATH
WORKDIR /src

COPY Directory.Build.props Directory.Packages.props ./
COPY src/ src/

RUN dotnet publish "${PROJECT_PATH}" \
        --configuration Release \
        --output /app/publish \
        /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
ARG ASSEMBLY_NAME
ARG APP_PORT=8080

ENV ASSEMBLY_NAME=${ASSEMBLY_NAME} \
    ASPNETCORE_HTTP_PORTS=${APP_PORT} \
    DOTNET_gcServer=1

# libgssapi-krb5-2: the RabbitMQ client resolves GSSAPI when opening a
#   connection, and the slim runtime image does not ship it, so without it every
#   broker connection fails with "libgssapi_krb5.so.2: cannot open shared object".
# curl: used by the container health probe; the image ships neither curl nor wget.
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE ${APP_PORT}
USER $APP_UID

ENTRYPOINT exec dotnet "${ASSEMBLY_NAME}.dll"
