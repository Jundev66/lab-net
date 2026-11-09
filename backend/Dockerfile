# Multi-stage build for ASP.NET Core (CodelyTV Modular DDD Architecture)
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /source

COPY LabNet.sln ./
COPY backend/src/Contexts/Shared/*.csproj backend/src/Contexts/Shared/
COPY backend/src/Contexts/Auth/*.csproj backend/src/Contexts/Auth/
COPY backend/src/Contexts/Projects/*.csproj backend/src/Contexts/Projects/
COPY backend/src/Apps/LabNet.WebApi/*.csproj backend/src/Apps/LabNet.WebApi/
COPY backend/tests/LabNet.UnitTests/*.csproj backend/tests/LabNet.UnitTests/

RUN dotnet restore backend/src/Apps/LabNet.WebApi/LabNet.WebApi.csproj

COPY backend/src/ backend/src/
WORKDIR /source/backend/src/Apps/LabNet.WebApi
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "LabNet.WebApi.dll"]
