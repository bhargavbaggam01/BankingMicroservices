# Stage 1 - build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY Banking.Gateway/*.csproj ./Banking.Gateway/
COPY Banking.Common/*.csproj ./Banking.Common/
RUN dotnet restore Banking.Gateway/Banking.Gateway.csproj

# Copy remaining files and publish
COPY . .
WORKDIR /src/Banking.Gateway
RUN dotnet publish -c Release -o /app/publish /p:PublishTrimmed=false

# Stage 2 - runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Banking.Gateway.dll"]
