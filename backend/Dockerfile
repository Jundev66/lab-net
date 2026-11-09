# Multi-stage build for ASP.NET Core
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /source

COPY LabNet.sln ./
COPY backend/src/LabNet.Domain/*.csproj backend/src/LabNet.Domain/
COPY backend/src/LabNet.Application/*.csproj backend/src/LabNet.Application/
COPY backend/src/LabNet.Infrastructure/*.csproj backend/src/LabNet.Infrastructure/
COPY backend/src/LabNet.WebApi/*.csproj backend/src/LabNet.WebApi/
COPY backend/tests/LabNet.UnitTests/*.csproj backend/tests/LabNet.UnitTests/

RUN dotnet restore backend/src/LabNet.WebApi/LabNet.WebApi.csproj

COPY backend/src/ backend/src/
WORKDIR /source/backend/src/LabNet.WebApi
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "LabNet.WebApi.dll"]
