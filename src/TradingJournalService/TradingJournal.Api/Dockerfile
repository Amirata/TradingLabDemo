FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/TradingJournalService/TradingJournal.Api/TradingJournal.Api.csproj", "src/TradingJournalService/TradingJournal.Api/"]
COPY ["src/TradingJournalService/TradingJournal.Application/TradingJournal.Application.csproj", "src/TradingJournalService/TradingJournal.Application/"]
COPY ["src/TradingJournalService/TradingJournal.Domain/TradingJournal.Domain.csproj", "src/TradingJournalService/TradingJournal.Domain/"]
COPY ["src/BuildingBlocks/BuildingBlocks.csproj", "src/BuildingBlocks/"]
COPY ["src/TradingJournalService/TradingJournal.Infrastructure/TradingJournal.Infrastructure.csproj", "src/TradingJournalService/TradingJournal.Infrastructure/"]
COPY ["src/Contracts/Contracts.csproj", "src/Contracts/"]
RUN dotnet restore "src/TradingJournalService/TradingJournal.Api/TradingJournal.Api.csproj"
COPY . .
WORKDIR "/src/src/TradingJournalService/TradingJournal.Api"
RUN dotnet build "TradingJournal.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TradingJournal.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

RUN apt-get update && apt-get install -y gosu && \
    rm -rf /var/lib/apt/lists/* && \
    mkdir -p /app/wwwroot/TechnicImages
COPY --from=publish /app/publish .
COPY --chmod=755 src/TradingJournalService/TradingJournal.Api/entrypoint.sh .
ENTRYPOINT ["./entrypoint.sh"]
