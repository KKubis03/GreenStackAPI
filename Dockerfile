FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["GreenStackAPI.csproj", "./"]
RUN dotnet restore "GreenStackAPI.csproj"
COPY . .
RUN dotnet build "GreenStackAPI.csproj" -c Release -o /app/build
RUN dotnet publish "GreenStackAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GreenStackAPI.dll"]
