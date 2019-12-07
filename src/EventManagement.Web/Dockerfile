FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["src/EventManagement.Web/EventManagement.Web.csproj", "src/EventManagement.Web/"]
COPY ["src/EventManagement.Domain/EventManagement.Domain.csproj", "src/EventManagement.Domain/"]
COPY ["src/EventManagement.Services/EventManagement.Services.csproj", "src/EventManagement.Services/"]
COPY ["src/EventManagement.Infrastructure/EventManagement.Infrastructure.csproj", "src/EventManagement.Infrastructure/"]
RUN dotnet restore "src/EventManagement.Web/EventManagement.Web.csproj"
COPY . .
WORKDIR "/src/src/EventManagement.Web"
RUN dotnet build "EventManagement.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventManagement.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "losol.EventManagement.Web.dll"]