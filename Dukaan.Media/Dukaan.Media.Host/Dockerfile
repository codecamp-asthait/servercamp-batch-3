FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Update package list and install the missing native graphics library
RUN apt-get update && apt-get install -y libfontconfig1 && rm -rf /var/lib/apt/lists/*


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy all 4 project files for dependency restore
COPY ["Dukaan.Media.Domain/Dukaan.Media.Domain.csproj", "Dukaan.Media.Domain/"]
COPY ["Dukaan.Media.Application/Dukaan.Media.Application.csproj", "Dukaan.Media.Application/"]
COPY ["Dukaan.Media.Infrastructure/Dukaan.Media.Infrastructure.csproj", "Dukaan.Media.Infrastructure/"]
COPY ["Dukaan.Media.Host/Dukaan.Media.Host.csproj", "Dukaan.Media.Host/"]

RUN dotnet restore "Dukaan.Media.Host/Dukaan.Media.Host.csproj"

COPY . .
WORKDIR "/src/Dukaan.Media.Host"
RUN dotnet build "Dukaan.Media.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dukaan.Media.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dukaan.Media.Host.dll"]
