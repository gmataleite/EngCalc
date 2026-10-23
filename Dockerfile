# =============================================================================
# Stage 1: Build & Publish (.NET 9 SDK - Root Context)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["backend/EngCalc.API/EngCalc.API.csproj", "EngCalc.API/"]
COPY ["backend/EngCalc.Application/EngCalc.Application.csproj", "EngCalc.Application/"]
COPY ["backend/EngCalc.Domain/EngCalc.Domain.csproj", "EngCalc.Domain/"]
COPY ["backend/EngCalc.Infrastructure/EngCalc.Infrastructure.csproj", "EngCalc.Infrastructure/"]

RUN dotnet restore "EngCalc.API/EngCalc.API.csproj"

COPY backend/ .
WORKDIR "/src/EngCalc.API"
RUN dotnet publish "EngCalc.API.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# =============================================================================
# Stage 2: Production Runtime (.NET 9 ASP.NET Runtime)
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet EngCalc.API.dll"]

