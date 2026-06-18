FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Dukaan.Host/Dukaan.Host.csproj", "Dukaan.Host/"]
COPY ["Dukaan.Application/Dukaan.Application.csproj", "Dukaan.Application/"]
COPY ["Dukaan.Infrastructure/Dukaan.Infrastructure.csproj", "Dukaan.Infrastructure/"]
COPY ["Dukaan.Domain/Dukaan.Domain.csproj", "Dukaan.Domain/"]
RUN dotnet restore "Dukaan.Host/Dukaan.Host.csproj"
COPY . .
WORKDIR "/src/Dukaan.Host"
RUN dotnet build "Dukaan.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dukaan.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dukaan.Host.dll"]
