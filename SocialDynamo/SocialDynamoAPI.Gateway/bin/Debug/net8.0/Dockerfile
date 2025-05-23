#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SocialDynamoAPI.Gateway/SocialDynamoAPI.Gateway.csproj", "SocialDynamoAPI.Gateway/"]
RUN dotnet restore "SocialDynamoAPI.Gateway/SocialDynamoAPI.Gateway.csproj"
COPY . .
WORKDIR "/src/SocialDynamoAPI.Gateway"
RUN dotnet build "SocialDynamoAPI.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SocialDynamoAPI.Gateway.csproj" -c Release -o /app/publish

# Copy the appsettings.json file
COPY ["Account/appsettings.json", "appsettings.json"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SocialDynamoAPI.Gateway.dll"]