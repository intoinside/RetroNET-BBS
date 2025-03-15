# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

# Restore and build project
COPY ["./Common/Common.csproj", "Common/"]
COPY ["./Encoder/Encoder.csproj", "Encoder/"]
COPY ["./Parser/Parser.csproj", "Parser/"]
COPY ["./PetsciiRawConverter/PetsciiRawConverter.csproj", "PetsciiRawConverter/"]
COPY ["./RetroNET-BBS/RetroNET-BBS.csproj", "RetroNET-BBS/"]
COPY ["./SampleDynamicContent/SampleDynamicContent.csproj", "SampleDynamicContent/"]

RUN dotnet restore "RetroNET-BBS/RetroNET-BBS.csproj"

RUN dotnet build "RetroNET-BBS/RetroNET-BBS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RetroNET-BBS/RetroNET-BBS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

EXPOSE 8502
EXPOSE 23

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetroNET-BBS.dll"]
