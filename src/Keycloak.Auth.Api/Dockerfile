FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Keycloak.Auth.Api/Keycloak.Auth.Api.csproj", "src/Keycloak.Auth.Api/"]
RUN dotnet restore "./src/Keycloak.Auth.Api/Keycloak.Auth.Api.csproj"
COPY . .
WORKDIR "/src/src/Keycloak.Auth.Api"
RUN dotnet build "./Keycloak.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Keycloak.Auth.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Keycloak.Auth.Api.dll"]