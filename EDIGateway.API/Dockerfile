FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AdventureWorks.EDI.WebAPI/AdventureWorks.EDI.WebAPI.csproj", "AdventureWorks.EDI.WebAPI/"]
RUN dotnet restore "AdventureWorks.EDI.WebAPI/AdventureWorks.EDI.WebAPI.csproj"
COPY . .
WORKDIR "/src/AdventureWorks.EDI.WebAPI"
RUN dotnet build "./AdventureWorks.EDI.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AdventureWorks.EDI.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdventureWorks.EDI.WebAPI.dll"]
