FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN groupadd -r mctech && useradd -r -g mctech mctech
WORKDIR /app
EXPOSE 8081
EXPOSE 443
ARG ALLOW_SWAGGER_UI=false
ARG ALLOW_ORIGINS=*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/WebApi", "WebApi"]
COPY ["src/Application", "Application"]
COPY ["src/CrossCutting.Ioc", "CrossCutting.Ioc"]
COPY ["src/ExternalServices", "ExternalServices"]
COPY ["src/Infra", "Infra"]
COPY ["src/Domain", "Domain"]
RUN dotnet restore "./WebApi/WebApi.csproj"
WORKDIR "/src/WebApi"
RUN dotnet build "./WebApi.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.csproj" \
    -c "$BUILD_CONFIGURATION" \
    -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER mctech
ENTRYPOINT ["dotnet", "WebApi.dll"]