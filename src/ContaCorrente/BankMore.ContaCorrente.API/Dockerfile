
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ContaCorrente/BankMore.ContaCorrente.API/BankMore.ContaCorrente.API.csproj", "src/ContaCorrente/BankMore.ContaCorrente.API/"]
COPY ["src/ContaCorrente/BankMore.ContaCorrente.Application/BankMore.ContaCorrente.Application.csproj", "src/ContaCorrente/BankMore.ContaCorrente.Application/"]
COPY ["src/ContaCorrente/BankMore.ContaCorrente.Domain/BankMore.ContaCorrente.Domain.csproj", "src/ContaCorrente/BankMore.ContaCorrente.Domain/"]
COPY ["src/ContaCorrente/BankMore.ContaCorrente.Infrastructure/BankMore.ContaCorrente.Infrastructure.csproj", "src/ContaCorrente/BankMore.ContaCorrente.Infrastructure/"]
RUN dotnet restore "./src/ContaCorrente/BankMore.ContaCorrente.API/BankMore.ContaCorrente.API.csproj"
COPY . .
WORKDIR "/src/src/ContaCorrente/BankMore.ContaCorrente.API"
RUN dotnet build "./BankMore.ContaCorrente.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankMore.ContaCorrente.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankMore.ContaCorrente.API.dll"]