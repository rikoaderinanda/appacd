# Step 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy csproj dan restore dependensi
COPY appacd/appacd.csproj ./appacd/
WORKDIR /src/appacd
RUN dotnet restore

# Copy semua file dan build
COPY appacd/. .
RUN dotnet publish -c Release -o /app

# Step 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /app .

EXPOSE 80
ENTRYPOINT ["dotnet", "appacd.dll"]
