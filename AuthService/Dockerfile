FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
ARG github_username
ARG github_pat
WORKDIR /src
COPY AuthService/AuthService.csproj AuthService/
RUN dotnet restore AuthService/AuthService.csproj
COPY . .
WORKDIR /src/AuthService
RUN dotnet build AuthService.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish AuthService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AuthService.dll"]