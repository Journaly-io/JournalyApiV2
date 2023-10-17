FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["JournalyApiV2/JournalyApiV2.csproj", "JournalyApiV2/"]
RUN dotnet restore "JournalyApiV2/JournalyApiV2.csproj"
COPY . .
WORKDIR "/src/JournalyApiV2"
RUN dotnet build "JournalyApiV2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JournalyApiV2.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JournalyApiV2.dll"]
