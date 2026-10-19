FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Notifications/Notifications.API/Notifications.API.csproj", "Notifications/Notifications.API/"]
COPY ["Notifications/Notifications.Application/Notifications.Application.csproj", "Notifications/Notifications.Application/"]
COPY ["Notifications/Notifications.Domain/Notifications.Domain.csproj", "Notifications/Notifications.Domain/"]
COPY ["Notifications/Notification.Shared/Notification.Shared.csproj", "Notifications/Notification.Shared/"]
COPY ["Notifications/Notifications.Infrastructure/Notifications.Infrastructure.csproj", "Notifications/Notifications.Infrastructure/"]
RUN dotnet restore "Notifications/Notifications.API/Notifications.API.csproj"
COPY . .
WORKDIR "/src/Notifications/Notifications.API"
RUN dotnet build "./Notifications.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Notifications.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifications.API.dll"]
