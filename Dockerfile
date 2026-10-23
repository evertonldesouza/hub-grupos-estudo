# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/HubGruposEstudo.Domain/HubGruposEstudo.Domain.csproj src/HubGruposEstudo.Domain/
COPY src/HubGruposEstudo.Application/HubGruposEstudo.Application.csproj src/HubGruposEstudo.Application/
COPY src/HubGruposEstudo.Infrastructure/HubGruposEstudo.Infrastructure.csproj src/HubGruposEstudo.Infrastructure/
COPY src/HubGruposEstudo.Web/HubGruposEstudo.Web.csproj src/HubGruposEstudo.Web/
RUN dotnet restore src/HubGruposEstudo.Web/HubGruposEstudo.Web.csproj

COPY src/ src/
RUN dotnet publish src/HubGruposEstudo.Web/HubGruposEstudo.Web.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HubGruposEstudo.Web.dll"]
