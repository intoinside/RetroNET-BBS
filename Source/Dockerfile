# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

ARG TARGETPLATFORM
ARG TARGETOS
ARG TARGETARCH
ARG TARGETVARIANT
ARG BUILDPLATFORM
ARG BUILDOS
ARG BUILDARCH
ARG BUILDVARIANT
RUN echo "Building on $BUILDPLATFORM, targeting $TARGETPLATFORM"
RUN echo "Building on ${BUILDOS} and ${BUILDARCH} with optional variant ${BUILDVARIANT}"
RUN echo "Targeting ${TARGETOS} and ${TARGETARCH} with optional variant ${TARGETVARIANT}"

# Restore and build project
COPY . .
RUN dotnet restore "RetroNET-BBS/RetroNET-BBS.csproj" -a $TARGETARCH
RUN dotnet build "RetroNET-BBS/RetroNET-BBS.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH --self-contained

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RetroNET-BBS/RetroNET-BBS.csproj" -c $BUILD_CONFIGURATION -o /app/publish -a $TARGETARCH --self-contained

EXPOSE 8502
EXPOSE 23

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetroNET-BBS.dll"]
