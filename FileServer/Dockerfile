FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./FileServer.csproj", "./"]
RUN dotnet restore "FileServer.csproj"
COPY . .
RUN dotnet build "FileServer.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "FileServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FileServer.dll"]
