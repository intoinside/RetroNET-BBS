# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

# Restore and build project
WORKDIR /src
COPY ["Source/RetroNET-BBS/RetroNET-BBS.csproj", "Source/RetroNET-BBS/"]
COPY ["Source/Common/Common.csproj", "Source/Common/"]
COPY ["Source/Encoder/Encoder.csproj", "Source/Encoder/"]
COPY ["Source/Parser/Parser.csproj", "Source/Parser/"]
RUN dotnet restore "./Source/RetroNET-BBS/RetroNET-BBS.csproj"

COPY . .
WORKDIR "/Source/RetroNET-BBS"
RUN dotnet build "./Source/RetroNET-BBS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Source/RetroNET-BBS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

EXPOSE 8502
EXPOSE 23

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetroNET-BBS.dll"]

