FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder
WORKDIR /src

RUN dotnet nuget add source https://nuget.nexdev.net/api/v2

COPY ["src/Fanex.Athena.CronJobs.Domain/Fanex.Athena.CronJobs.Domain.csproj", "./Fanex.Athena.CronJobs.Domain/"]
COPY ["src/Fanex.Athena.CronJobs.Infrastructure/Fanex.Athena.CronJobs.Infrastructure.csproj", "./Fanex.Athena.CronJobs.Infrastructure/"]
COPY ["src/Fanex.Athena.CronJobs.Application/Fanex.Athena.CronJobs.Application.csproj", "./Fanex.Athena.CronJobs.Application/"]
COPY ["src/Fanex.Athena.CronJobs.Host/Fanex.Athena.CronJobs.Host.csproj", "./Fanex.Athena.CronJobs.Host/"]

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore "Fanex.Athena.CronJobs.Host/Fanex.Athena.CronJobs.Host.csproj" -v n

COPY src .

RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish "Fanex.Athena.CronJobs.Host/Fanex.Athena.CronJobs.Host.csproj" \
    -c Release -o /app/publish --self-contained false

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine-25h1 AS final

ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Dont run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app/publish .
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Athena.CronJobs.Host.dll"]