FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY GoFish.sln ./
COPY src/GoFish.Core/GoFish.Core.csproj src/GoFish.Core/
COPY src/GoFish.Application/GoFish.Application.csproj src/GoFish.Application/
COPY src/GoFish.Api/GoFish.Api.csproj src/GoFish.Api/
COPY tests/GoFish.Core.Tests/GoFish.Core.Tests.csproj tests/GoFish.Core.Tests/

RUN dotnet restore GoFish.sln

COPY . .
RUN dotnet publish src/GoFish.Api/GoFish.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
ENV GOFISH_WEB_ROOT=/app/web

COPY --from=build /app/publish ./
COPY web ./web

EXPOSE 8080
ENTRYPOINT ["dotnet", "GoFish.Api.dll"]
