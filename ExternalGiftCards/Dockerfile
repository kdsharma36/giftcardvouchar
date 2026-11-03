# TODO: confirm the .NET 10 runtime tag with DevOps (DynamicPricing still references :7)
FROM nexus.eventim.net:5043/inhouse/dotnet-aspnet:10 AS base
ARG BUILD_VERSION=0.0.0.1-main
ARG BUILD_DATE=1900-01-01T00:00:01Z
ARG VCS_REF=0000000000000000000000000000000000000000

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

COPY . .
WORKDIR "/src/ExternalGiftCards"
RUN dotnet restore --source https://nexus.eventim.net/repository/nuget-group/
RUN dotnet build "ExternalGiftCards.csproj" -c Release -o /app/build

FROM build AS publish
ARG version
ARG vc_revision
RUN dotnet publish "ExternalGiftCards.csproj" -c Release -o /app/publish /p:Version="$version" /p:SourceRevisionId="$vc_revision"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER 1000
EXPOSE 8080
ENV ASPNETCORE_URLS="http://+:8080"
ENV BasePath=/external-gift-cards/

ENTRYPOINT ["dotnet", "ExternalGiftCards.dll"]
