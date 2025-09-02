FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files
COPY src/PantryCloud.IdentityService.Presentation/PantryCloud.IdentityService.Presentation.csproj src/PantryCloud.IdentityService.Presentation/
COPY src/PantryCloud.IdentityService.Application/PantryCloud.IdentityService.Application.csproj src/PantryCloud.IdentityService.Application/
COPY src/PantryCloud.IdentityService.Core/PantryCloud.IdentityService.Core.csproj src/PantryCloud.IdentityService.Core/
COPY src/PantryCloud.IdentityService.Infrastructure/PantryCloud.IdentityService.Infrastructure.csproj src/PantryCloud.IdentityService.Infrastructure/

# Restore NuGet packages
RUN dotnet restore src/PantryCloud.IdentityService.Presentation/PantryCloud.IdentityService.Presentation.csproj

# Copy the rest of the source code
COPY . .

COPY secrets /secrets

# Build 
WORKDIR /src/src/PantryCloud.IdentityService.Presentation
RUN dotnet build PantryCloud.IdentityService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/build

# Publish 
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish PantryCloud.IdentityService.Presentation.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY secrets /secrets

ENTRYPOINT ["dotnet", "PantryCloud.IdentityService.Presentation.dll"]