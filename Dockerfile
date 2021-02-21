FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS build-env

WORKDIR /source

COPY src/RecipesAPI.API/RecipesAPI.API.csproj src/RecipesAPI.API/
COPY src/RecipesAPI.Application/RecipesAPI.Application.csproj src/RecipesAPI.Application/
COPY src/RecipesAPI.Domain/RecipesAPI.Domain.csproj src/RecipesAPI.Domain/
COPY src/RecipesAPI.Infrastructure/RecipesAPI.Infrastructure.csproj src/RecipesAPI.Infrastructure/
COPY src/RecipesAPI.Tests/RecipesAPI.Tests.csproj src/RecipesAPI.Tests/
COPY RecipesAPI.sln .
RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /build src/RecipesAPI.API/RecipesAPI.API.csproj

FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app

ENV ASPNETCORE_URLS=https://+,http://+
EXPOSE 80/tcp
EXPOSE 443/tcp

COPY --from=build-env /build .
ENTRYPOINT ["dotnet", "RecipesAPI.API.dll"]