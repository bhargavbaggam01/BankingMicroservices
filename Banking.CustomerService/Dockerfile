# Stage 1 - build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY Banking.CustomerService/*.csproj ./Banking.CustomerService/
COPY Banking.Common/*.csproj ./Banking.Common/
RUN dotnet restore Banking.CustomerService/Banking.CustomerService.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/Banking.CustomerService
RUN dotnet publish -c Release -o /app/publish /p:PublishTrimmed=false

# Stage 2 - runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Banking.CustomerService.dll"]
