FROM hub.nexdev.net/base/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder
WORKDIR /src
COPY ["NuGet.Config", "."]
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2

COPY ["src/Fanex.Athena.CronJobs.Core/Fanex.Athena.CronJobs.Core.csproj", "./Fanex.Athena.CronJobs.Core/"]
RUN dotnet restore "Fanex.Athena.CronJobs.Core/Fanex.Athena.CronJobs.Core.csproj" -v n

COPY ["src/Fanex.Athena.CronJobs.Infrastructure/Fanex.Athena.CronJobs.Infrastructure.csproj", "./Fanex.Athena.CronJobs.Infrastructure/"]
RUN dotnet restore "Fanex.Athena.CronJobs.Infrastructure/Fanex.Athena.CronJobs.Infrastructure.csproj" -v n

COPY ["src/Fanex.Athena.CronJobs.App/Fanex.Athena.CronJobs.App.csproj", "./Fanex.Athena.CronJobs.App/"]
RUN dotnet restore "Fanex.Athena.CronJobs.App/Fanex.Athena.CronJobs.App.csproj" -v n

COPY ["src/Fanex.Athena.CronJobs.Site/Fanex.Athena.CronJobs.Site.csproj", "./Fanex.Athena.CronJobs.Site/"]
RUN dotnet restore "Fanex.Athena.CronJobs.Site/Fanex.Athena.CronJobs.Site.csproj" -v n

COPY src .
WORKDIR "/src/Fanex.Athena.CronJobs.Site"
RUN dotnet build "Fanex.Athena.CronJobs.Site.csproj" -c Release -o /app/build

RUN dotnet publish "Fanex.Athena.CronJobs.Site.csproj" -c Release -o /app/publish

FROM hub.nexdev.net/base/dotnet/core/aspnet:5.0-alpine AS base

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
WORKDIR /app

FROM base AS final
WORKDIR /app
COPY --from=builder /app/publish .
ENTRYPOINT ["dotnet", "Fanex.Athena.CronJobs.Site.dll"]