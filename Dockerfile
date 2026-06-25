FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["WhatchParty.csproj", "."]
RUN dotnet restore "./WhatchParty.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WhatchParty.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WhatchParty.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WhatchParty.dll"]
