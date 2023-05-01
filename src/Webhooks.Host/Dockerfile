FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Nuget.Config", "."]
COPY ["src/Webhooks.Host/Webhooks.Host.csproj", "src/Webhooks.Host/"]
COPY ["src/Webhooks.BW/Webhooks.BW.csproj", "src/Webhooks.BW/"]
COPY ["src/Webhooks.Engine/Webhooks.Engine.csproj", "src/Webhooks.Engine/"]
COPY ["src/Webhooks.Commands/Webhooks.Commands.csproj", "src/Webhooks.Commands/"]
RUN dotnet restore "Webhooks.Host/Webhooks.Host.csproj"
COPY . .
WORKDIR "/src/Webhooks.Host"
RUN dotnet build "Webhooks.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Webhooks.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Webhooks.Host.dll"]