#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Account/Account.API.csproj", "Account/"]
COPY ["Account.Domain/Account.Domain.csproj", "Account.Domain/"]
COPY ["Account.Infrastructure/Account.Infrastructure.csproj", "Account.Infrastructure/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Account/Account.API.csproj"
COPY . .
WORKDIR "/src/Account"
RUN dotnet build "Account.API.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "Account.API.csproj" -c Release -o /app/publish

# Copy the appsettings.json file
COPY ["Account/appsettings.json", "appsettings.json"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Account.API.dll"]