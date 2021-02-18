#pick sdk
FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build-env

WORKDIR /source

#copy files

COPY ShoppingListApi.sln .
RUN dotnet restore

COPY . .

#add main project
RUN dotnet publish -c Release -o 

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app

ENV ASPNETCORE_URLS=https://+,http://+
EXPOSE 80/tcp
EXPOSE 443/tcp

COPY --from=build-env /build .
ENTRYPOINT ["dotnet", ""]