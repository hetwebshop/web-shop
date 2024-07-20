FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY API/API.csproj .
RUN dotnet restore API.csproj
COPY API .
RUN dotnet build "API.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]