FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Web/Config/Config.WebMVC/Config.WebMVC.csproj Web/Config/Config.WebMVC/
RUN dotnet restore Web/Config/Config.WebMVC/Config.WebMVC.csproj
COPY . .
WORKDIR /src/Web/Config/Config.WebMVC
RUN dotnet build Config.WebMVC.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Config.WebMVC.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Config.WebMVC.dll"]
