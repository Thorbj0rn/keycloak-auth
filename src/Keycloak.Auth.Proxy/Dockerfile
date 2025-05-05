FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/Keycloak.Auth.Proxy/Keycloak.Auth.Proxy.csproj", "src/Keycloak.Auth.Proxy/"]
RUN dotnet restore "./src/Keycloak.Auth.Proxy/Keycloak.Auth.Proxy.csproj"
COPY . .
WORKDIR "/src/src/Keycloak.Auth.Proxy"
RUN dotnet build "./Keycloak.Auth.Proxy.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Keycloak.Auth.Proxy.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Keycloak.Auth.Proxy.dll"]