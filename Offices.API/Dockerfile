FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Offices/Offices.API/Offices.API.csproj", "Offices/Offices.API/"]
COPY ["Offices/Offices.Infrastructure/Offices.Infrastructure.csproj", "Offices/Offices.Infrastructure/"]
COPY ["Offices/Offices.Application/Offices.Application.csproj", "Offices/Offices.Application/"]
COPY ["Offices/Offices.Domain/Offices.Domain.csproj", "Offices/Offices.Domain/"]
RUN dotnet restore "Offices/Offices.API/Offices.API.csproj"
COPY . .
WORKDIR "/src/Offices/Offices.API"
RUN dotnet build "./Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Offices.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Offices.API.dll"]
