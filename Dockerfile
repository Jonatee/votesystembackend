# ----------------------------------------------------------------------
# Stage 0: BASE - Minimal runtime image for the final production layer
# ----------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# Use a non-root user for enhanced security (Good practice)
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ----------------------------------------------------------------------
# Stage 1: BUILD - Optimized compilation with caching
# ----------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Restore Dependencies (The Caching Magic)
# Copy ONLY the project file first to cache the 'dotnet restore' layer.
# This step only re-runs if the project file (which lists dependencies) changes.
COPY ["votesystembackend.csproj", "."]
RUN dotnet restore

# 2. Copy Remaining Source Code
# Copy the rest of the source code (which changes frequently)
COPY . .

# 3. Build the Application
RUN dotnet build "votesystembackend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ----------------------------------------------------------------------
# Stage 2: PUBLISH - Create the minimal deployment package
# ----------------------------------------------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Use --no-build since the build was already done in the previous stage
RUN dotnet publish "votesystembackend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-build

# ----------------------------------------------------------------------
# Stage 3: FINAL - The secure, minimal runtime image
# ----------------------------------------------------------------------
FROM base AS final
WORKDIR /app
# Copy only the published output from the publish stage
COPY --from=publish /app/publish .

# Set the final execution command
ENTRYPOINT ["dotnet", "votesystembackend.dll"]