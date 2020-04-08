#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["WeatherForecast/WeatherForecast.csproj", "WeatherForecast/"]
RUN dotnet restore "WeatherForecast/WeatherForecast.csproj"
COPY . .
WORKDIR "/src/WeatherForecast"
RUN dotnet build "WeatherForecast.csproj" -c Release -o /app/build
WORKDIR "/src/WeatherForecast.UnitTests"
RUN dotnet vstest "WeatherForecast.UnitTests.csproj" --logger:"trx"

FROM build AS publish
WORKDIR "/src/WeatherForecast"
RUN dotnet publish "WeatherForecast.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherForecast.dll"]