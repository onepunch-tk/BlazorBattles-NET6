FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Server/BlazorBattles.Server.csproj", "Server/"]
COPY ["Client/BlazorBattles.Client.csproj", "Client/"]
COPY ["Shared/BlazorBattles.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/BlazorBattles.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "BlazorBattles.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorBattles.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorBattles.Server.dll"]
