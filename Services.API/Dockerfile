FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Services.API/Services.API.csproj", "Services/Services.API/"]
COPY ["Services/Services.Application/Services.Application.csproj", "Services/Services.Application/"]
COPY ["Services/Services.Domain/Services.Domain.csproj", "Services/Services.Domain/"]
COPY ["Services/Services.Infrastructure/Services.Infrastructure.csproj", "Services/Services.Infrastructure/"]
RUN dotnet restore "Services/Services.API/Services.API.csproj"
COPY . .
WORKDIR "/src/Services/Services.API"
RUN dotnet build "./Services.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Services.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Services.API.dll"]
