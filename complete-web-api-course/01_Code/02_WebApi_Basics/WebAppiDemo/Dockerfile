# Use the official .NET 10 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET SDK for building the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["WebApiDemo.csproj", "."]
RUN dotnet restore "./WebApiDemo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WebApiDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiDemo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiDemo.dll"]
