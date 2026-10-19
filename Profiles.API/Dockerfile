FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Profiles/Profiles.API/Profiles.API.csproj", "Profiles/Profiles.API/"]
COPY ["Profiles/Profiles.Infrastructure/Profiles.Infrastructure.csproj", "Profiles/Profiles.Infrastructure/"]
COPY ["Profiles/Profiles.Application/Profiles.Application.csproj", "Profiles/Profiles.Application/"]
COPY ["Profiles/Profiles.Domain/Profiles.Domain.csproj", "Profiles/Profiles.Domain/"]
COPY ["Profiles/Profiles.Shared/Profiles.Shared.csproj", "Profiles/Profiles.Shared/"]
RUN dotnet restore "Profiles/Profiles.API/Profiles.API.csproj"
COPY . .
WORKDIR "/src/Profiles/Profiles.API"
RUN dotnet build "./Profiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Profiles.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Profiles.API.dll"]
