FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY . .

RUN dotnet publish -c Release -o /app/publish

WORKDIR /app/publish

ENTRYPOINT ["dotnet", "learning-docker.dll"]
