FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Consommateur.Kafka/Consommateur.Kafka.csproj", "Consommateur.Kafka/"]
RUN dotnet restore "Consommateur.Kafka/Consommateur.Kafka.csproj"
COPY . .
WORKDIR "/src/Consommateur.Kafka"
RUN dotnet build "Consommateur.Kafka.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Consommateur.Kafka.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Consommateur.Kafka.dll"]
