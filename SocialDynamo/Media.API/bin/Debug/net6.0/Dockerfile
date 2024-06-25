#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Media.API/Media.API.csproj", "Media.API/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Media.API/Media.API.csproj"
COPY . .
WORKDIR "/src/Media.API"
RUN dotnet build "Media.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Media.API.csproj" -c Release -o /app/publish

# Copy the appsettings.json file
COPY ["Account/appsettings.json", "appsettings.json"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Media.API.dll"]