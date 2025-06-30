# Step 1: Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy csproj and restore
COPY app.acd.csproj ./
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
ENTRYPOINT ["dotnet", "app.acd.dll"]