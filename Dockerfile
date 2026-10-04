FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY global.json ./
COPY src/Devices.Domain/Devices.Domain.csproj src/Devices.Domain/
COPY src/Devices.Application/Devices.Application.csproj src/Devices.Application/
COPY src/Devices.Infrastructure/Devices.Infrastructure.csproj src/Devices.Infrastructure/
COPY src/Devices.Api/Devices.Api.csproj src/Devices.Api/
RUN dotnet restore src/Devices.Api/Devices.Api.csproj

COPY src/ src/
RUN dotnet publish src/Devices.Api/Devices.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "Devices.Api.dll"]
