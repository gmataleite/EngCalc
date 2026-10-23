# =============================================================================
# Stage 1: Build & Publish (.NET 9 SDK)
# =============================================================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first to leverage Docker layer caching
COPY ["EngCalc.API/EngCalc.API.csproj", "EngCalc.API/"]
COPY ["EngCalc.Application/EngCalc.Application.csproj", "EngCalc.Application/"]
COPY ["EngCalc.Domain/EngCalc.Domain.csproj", "EngCalc.Domain/"]
COPY ["EngCalc.Infrastructure/EngCalc.Infrastructure.csproj", "EngCalc.Infrastructure/"]

RUN dotnet restore "EngCalc.API/EngCalc.API.csproj"

# Copy remaining backend source code and publish
COPY . .
WORKDIR "/src/EngCalc.API"
RUN dotnet publish "EngCalc.API.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

# =============================================================================
# Stage 2: Production Runtime (.NET 9 ASP.NET Runtime)
# =============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published output (includes Data/Seed/*.csv)
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

# Run as built-in non-root user for container security
USER $APP_UID

# Bind dynamically to Render's $PORT (or 8080 locally)
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080} exec dotnet EngCalc.API.dll"]

