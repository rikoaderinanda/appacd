# Step 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /src

# Copy csproj and restore
COPY appacd/appacd.csproj ./appacd/
WORKDIR /src/appacd
RUN dotnet restore

# Copy the rest of the app
COPY appacd/. .

# Publish the app
RUN dotnet publish -c Release -o /app

# Step 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /out .

EXPOSE 80
ENTRYPOINT ["dotnet", "appacd.dll"]