#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SocialDynamoAPI/SocialDynamoAPI.BaseAggregator.csproj", "SocialDynamoAPI/"]
RUN dotnet restore "SocialDynamoAPI/SocialDynamoAPI.BaseAggregator.csproj"
COPY . .
WORKDIR "/src/SocialDynamoAPI"
RUN dotnet build "SocialDynamoAPI.BaseAggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialDynamoAPI.BaseAggregator.csproj" -c Release -o /app/publish

# Copy the appsettings.json file
COPY ["SocialDynamoAPI/appsettings.json", "appsettings.json"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialDynamoAPI.BaseAggregator.dll"]