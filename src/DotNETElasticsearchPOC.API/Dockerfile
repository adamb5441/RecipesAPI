#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/DotNETElasticsearchPOC.API/DotNETElasticsearchPOC.API.csproj", "src/DotNETElasticsearchPOC.API/"]
COPY ["src/DotNETElasticsearchPOC.Application/DotNETElasticsearchPOC.Application.csproj", "src/DotNETElasticsearchPOC.Application/"]
COPY ["src/DotNETElasticsearchPOC.Tests/DotNETElasticsearchPOC.Tests.csproj", "src/DotNETElasticsearchPOC.Tests/"]
RUN dotnet restore "src/DotNETElasticsearchPOC.API/DotNETElasticsearchPOC.API.csproj"
COPY . .
WORKDIR "/src/src/DotNETElasticsearchPOC.API"
RUN dotnet build "DotNETElasticsearchPOC.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotNETElasticsearchPOC.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotNETElasticsearchPOC.API.dll"]
