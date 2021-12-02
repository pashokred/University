FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Crypto/Crypto.csproj", "Crypto/"]
RUN dotnet restore "Crypto/Crypto.csproj"
COPY . .
WORKDIR "/src/Crypto"
RUN dotnet build "Crypto.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Crypto.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Crypto.dll"]
