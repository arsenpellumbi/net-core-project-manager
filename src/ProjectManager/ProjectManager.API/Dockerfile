FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advanprojectTaske of Docker's build cache, to speed up local contPigeonner builds
COPY "sln" "sln"

COPY ["ProjectManager/ProjectManager.API/ProjectManager.API.csproj", "ProjectManager/ProjectManager.API/"]
COPY ["ProjectManager/ProjectManager.Infrastructure.Persistence.PostgreSQL/ProjectManager.Infrastructure.Persistence.PostgreSQL.csproj", "ProjectManager/ProjectManager.Infrastructure.Persistence.PostgreSQL/"]
COPY ["ProjectManager/ProjectManager.Core/ProjectManager.Core.csproj", "ProjectManager/ProjectManager.Core/"]
COPY ["ProjectManager/ProjectManager.Queries/ProjectManager.Queries.csproj", "ProjectManager/ProjectManager.Queries/"]
COPY ["ProjectManager/ProjectManager.Commands/ProjectManager.Commands.csproj", "ProjectManager/ProjectManager.Commands/"]

COPY "docker-compose.dcproj" "docker-compose.dcproj"

COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "sln"

COPY . .
WORKDIR "/src/ProjectManager/ProjectManager.API"
RUN dotnet build "ProjectManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProjectManager.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProjectManager.API.dll"]
