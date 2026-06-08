# Stage 1: build - uses the full .NET SDK image to compile and publish the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# Set the working directory inside the container
WORKDIR /src
# Copy all project files into the container
COPY . .
# Publish the app in Release mode, output to /app/publish
RUN dotnet publish -c Release -o /app/publish

# Stage 2: runtime - uses the lightweight ASP.NET runtime image (no SDK)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
# Set the working directory for the final image
WORKDIR /app
# Copy only the published output from the build stage
COPY --from=build /app/publish .
# Run the application when the container starts
ENTRYPOINT ["dotnet", "learning-docker.dll"]
