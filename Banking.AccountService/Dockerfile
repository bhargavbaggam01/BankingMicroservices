# Stage 1 - build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY Banking.AccountService/*.csproj ./Banking.AccountService/
COPY Banking.Common/*.csproj ./Banking.Common/
RUN dotnet restore Banking.AccountService/Banking.AccountService.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/Banking.AccountService
RUN dotnet publish -c Release -o /app/publish /p:PublishTrimmed=false

# Stage 2 - runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5002
EXPOSE 5002
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Banking.AccountService.dll"]
