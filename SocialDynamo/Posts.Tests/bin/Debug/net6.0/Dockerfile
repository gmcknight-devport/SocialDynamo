#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Posts.API/Posts.API.csproj", "Posts.API/"]
COPY ["Posts.Infrastructure/Posts.Infrastructure.csproj", "Posts.Infrastructure/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Posts.Domain/Posts.Domain.csproj", "Posts.Domain/"]
RUN dotnet restore "Posts.API/Posts.API.csproj"
COPY . .
WORKDIR "/src/Posts.API"
RUN dotnet build "Posts.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Posts.API.csproj" -c Release -o /app/publish

# Copy the appsettings.json file
COPY ["Account/appsettings.json", "appsettings.json"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Posts.API.dll"]