#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["tdb.ddd.files.webapi/tdb.ddd.files.webapi.csproj", "tdb.ddd.files.webapi/"]
RUN dotnet restore "tdb.ddd.files.webapi/tdb.ddd.files.webapi.csproj"
COPY . .
WORKDIR "/src/tdb.ddd.files.webapi"
RUN dotnet build "tdb.ddd.files.webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "tdb.ddd.files.webapi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "tdb.ddd.files.webapi.dll"]