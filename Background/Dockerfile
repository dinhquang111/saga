FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Background.csproj", "./"]
RUN dotnet restore "Background.csproj"
COPY . .
RUN dotnet build "Background.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Background.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Background.dll"]
