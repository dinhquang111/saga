FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Foreground.csproj", "./"]
RUN dotnet restore "Foreground.csproj"
COPY . .
RUN dotnet build "Foreground.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish "Foreground.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Foreground.dll"]
