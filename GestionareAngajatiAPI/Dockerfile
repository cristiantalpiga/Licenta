# Runtime cu .NET 9
# FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# SDK pentru build cu .NET 9
# FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Imagine finală
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GestionareAngajatiAPI.dll"]
