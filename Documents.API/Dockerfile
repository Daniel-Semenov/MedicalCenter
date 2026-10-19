FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Documents/Documents.API/Documents.API.csproj", "Documents/Documents.API/"]
COPY ["Documents/Documents.Shared/Documents.Shared.csproj", "Documents/Documents.Shared/"]
COPY ["Documents/Documents.Domain/Documents.Domain.csproj", "Documents/Documents.Domain/"]
COPY ["Documents/Documents.Contracts/Documents.Contracts.csproj", "Documents/Documents.Contracts/"]
COPY ["Documents/Documents.Application/Documents.Application.csproj", "Documents/Documents.Application/"]
COPY ["Documents/Documents.Infrastructure/Documents.Infrastructure.csproj", "Documents/Documents.Infrastructure/"]
RUN dotnet restore "Documents/Documents.API/Documents.API.csproj"
COPY . .
WORKDIR "/src/Documents/Documents.API"
RUN dotnet build "./Documents.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Documents.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Documents.API.dll"]
