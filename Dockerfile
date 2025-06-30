# # Step 1: Build
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# WORKDIR /src
# COPY *.csproj .
# RUN dotnet restore
# COPY . .
# RUN dotnet publish -c Release -o /app

# # Step 2: Runtime
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# WORKDIR /app
# COPY --from=build /app .
# EXPOSE 80
# ENTRYPOINT ["dotnet", "appacd.dll"]

# Step 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy csproj and restore
COPY appacd.csproj ./
RUN dotnet restore

# Copy rest of the code
COPY . ./

# Publish the app
RUN dotnet publish -c Release -o /out

# Step 2: Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /out .

EXPOSE 80
ENTRYPOINT ["dotnet", "appacd.dll"]
