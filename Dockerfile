FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt update
RUN apt-get update
RUN apt install gettext-base  -y
RUN apt-get install tzdata && \
    ln -fs /usr/share/zoneinfo/America/New_York /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata
ENV TZ="Asia/Baku"
WORKDIR /app
COPY ["appsettings.json", "/app/appsettings.txt"]
EXPOSE 80
ENV ASPNETCORE_HTTP_PORTS=80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DigitalSalaryService.csproj", "."]
RUN dotnet restore "./DigitalSalaryService.csproj" -s http://linux-repo.accessbank.local/nuget/v3/index.json
COPY . .
WORKDIR "/src/."
RUN dotnet build "./DigitalSalaryService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DigitalSalaryService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalSalaryService.dll"]
