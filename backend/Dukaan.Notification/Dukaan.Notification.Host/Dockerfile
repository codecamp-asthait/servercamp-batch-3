FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Dukaan.Notification.slnx ./
COPY Dukaan.Notification.Domain/Dukaan.Notification.Domain.csproj Dukaan.Notification.Domain/
COPY Dukaan.Notification.Application/Dukaan.Notification.Application.csproj Dukaan.Notification.Application/
COPY Dukaan.Notification.Infrastructure/Dukaan.Notification.Infrastructure.csproj Dukaan.Notification.Infrastructure/
COPY Dukaan.Notification.Host/Dukaan.Notification.Host.csproj Dukaan.Notification.Host/

RUN dotnet restore Dukaan.Notification.Host/Dukaan.Notification.Host.csproj

COPY . .
RUN dotnet publish Dukaan.Notification.Host/Dukaan.Notification.Host.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Dukaan.Notification.Host.dll"]
