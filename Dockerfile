FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish LibraryApi/LibraryApi.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY LegacyDatabase/legacy_schema.sql ./
COPY LegacyDatabase/legacy_sample_data.sql ./
ENTRYPOINT ["dotnet", "LibraryApi.dll"]