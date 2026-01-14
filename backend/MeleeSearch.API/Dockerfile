FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MeleeSearch.APi/MeleeSearch.APi.csproj", "MeleeSearch.APi/"]
RUN dotnet restore "MeleeSearch.APi/MeleeSearch.APi.csproj"
COPY . .
WORKDIR "/src/MeleeSearch.APi"
RUN dotnet build "./MeleeSearch.APi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MeleeSearch.APi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeleeSearch.APi.dll"]
