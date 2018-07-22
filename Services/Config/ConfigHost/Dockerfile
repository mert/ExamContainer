FROM microsoft/dotnet:2.1-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Services/Config/ConfigHost/ConfigHost.csproj Services/Config/ConfigHost/
RUN dotnet restore Services/Config/ConfigHost/ConfigHost.csproj
COPY . .
WORKDIR /src/Services/Config/ConfigHost
RUN dotnet build ConfigHost.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ConfigHost.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ConfigHost.dll"]
