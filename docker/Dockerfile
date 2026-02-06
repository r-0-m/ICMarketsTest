# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Api/ICMarketsTest.Api.csproj", "src/Api/"]
COPY ["src/Core/ICMarketsTest.Core.csproj", "src/Core/"]
COPY ["src/Contracts/ICMarketsTest.Contracts.csproj", "src/Contracts/"]
COPY ["src/Infrastructure/ICMarketsTest.Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Api/ICMarketsTest.Api.csproj"
COPY . .
WORKDIR "/src/src/Api"
RUN dotnet build "ICMarketsTest.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ICMarketsTest.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
ENV Database__FilePath=/app/sql/blockchain.db
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
RUN mkdir -p /app/sql
COPY --from=publish /app/publish .
HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -fsS http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "ICMarketsTest.Api.dll"]