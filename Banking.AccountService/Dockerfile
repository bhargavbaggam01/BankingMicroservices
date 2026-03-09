FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore Banking.AccountService/Banking.AccountService.csproj
RUN dotnet publish Banking.AccountService/Banking.AccountService.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app .

ENTRYPOINT ["dotnet", "Banking.AccountService.dll"]