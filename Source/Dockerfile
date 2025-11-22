# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#docker run -v /opt/bbs:/data:ro -p 8502:8502 ghcr.io/intoinside/retronet-bbs:main
# This stage is used to build the service project
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build

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

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RetroNET-BBS/RetroNET-BBS.csproj" -c $BUILD_CONFIGURATION -o /app/publish -a $TARGETARCH --self-contained

EXPOSE 8502
EXPOSE 23

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app
USER $APP_UID
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetroNET-BBS.dll"]
