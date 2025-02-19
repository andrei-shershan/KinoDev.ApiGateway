FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and projects
COPY ["KinoDev.ApiGateway.sln", "./"]
COPY ["src/KinoDev.ApiGateway.WebApi/KinoDev.ApiGateway.WebApi.csproj", "src/KinoDev.ApiGateway.WebApi/"]
COPY ["tests/KinoDev.ApiGateway.UnitTests/KinoDev.ApiGateway.UnitTests.csproj", "tests/KinoDev.ApiGateway.UnitTests/"]
RUN dotnet restore "src/KinoDev.ApiGateway.WebApi/KinoDev.ApiGateway.WebApi.csproj"

# Copy full source and build
COPY . .
WORKDIR "/src/src/KinoDev.ApiGateway.WebApi"
RUN dotnet build "KinoDev.ApiGateway.WebApi.csproj" -c Release -o /app/build

# Add a stage for running tests
# FROM build AS testrunner
# WORKDIR /src/tests/FoodExpress.Api.UnitTests
# RUN dotnet test "FoodExpress.Api.UnitTests.csproj" --logger:trx

FROM build AS publish
RUN dotnet publish "KinoDev.ApiGateway.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KinoDev.ApiGateway.WebApi.dll"]
