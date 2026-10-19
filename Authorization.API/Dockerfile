FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Authorization/Authorization.API/Authorization.API.csproj", "Authorization/Authorization.API/"]
COPY ["Authorization/Authorization.Application/Authorization.Application.csproj", "Authorization/Authorization.Application/"]
COPY ["Authorization/Authorization.Domain/Authorization.Domain.csproj", "Authorization/Authorization.Domain/"]
COPY ["Authorization/Authorization.Shared/Authorization.Shared.csproj", "Authorization/Authorization.Shared/"]
COPY ["Authorization/Authorization.Infrastructure/Authorization.Infrastructure.csproj", "Authorization/Authorization.Infrastructure/"]
RUN dotnet restore "Authorization/Authorization.API/Authorization.API.csproj"
COPY . .
WORKDIR "/src/Authorization/Authorization.API"
RUN dotnet build "./Authorization.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Authorization.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authorization.API.dll"]
