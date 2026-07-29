# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia só os .csproj primeiro pra aproveitar cache de camadas do Docker
COPY src/DevFlow.Domain/*.csproj src/DevFlow.Domain/
COPY src/DevFlow.Application/*.csproj src/DevFlow.Application/
COPY src/DevFlow.Infrastructure/*.csproj src/DevFlow.Infrastructure/
COPY src/DevFlow.Api/*.csproj src/DevFlow.Api/
RUN dotnet restore src/DevFlow.Api/DevFlow.Api.csproj

# Agora copia o resto do código e publica
COPY src/ src/
RUN dotnet publish src/DevFlow.Api/DevFlow.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "DevFlow.Api.dll"]
