FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Appointments/Appointments.API/Appointments.API.csproj", "Appointments/Appointments.API/"]
COPY ["Appointments/Appointments.Application/Appointments.Application.csproj", "Appointments/Appointments.Application/"]
COPY ["Appointments/Appointments.Domain/Appointments.Domain.csproj", "Appointments/Appointments.Domain/"]
COPY ["Appointments/Appointments.Shared/Appointments.Shared.csproj", "Appointments/Appointments.Shared/"]
COPY ["Appointments/Appointments.Infrastructure/Appointments.Infrastructure.csproj", "Appointments/Appointments.Infrastructure/"]
RUN dotnet restore "Appointments/Appointments.API/Appointments.API.csproj"
COPY . .
WORKDIR "/src/Appointments/Appointments.API"
RUN dotnet build "./Appointments.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Appointments.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Appointments.API.dll"]
