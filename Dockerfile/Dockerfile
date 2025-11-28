# Use the official .NET image as a build environment
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY LibraryApi/*.csproj ./LibraryApi/
RUN dotnet restore ./LibraryApi/LibraryApi.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ./LibraryApi/LibraryApi.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 8080 for Render
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LibraryApi.dll"]
