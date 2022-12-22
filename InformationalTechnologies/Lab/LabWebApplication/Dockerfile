FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["LabWebApplication/LabWebApplication.csproj", "LabWebApplication/"]
RUN dotnet restore "LabWebApplication/LabWebApplication.csproj"
COPY . .
WORKDIR "/src/LabWebApplication"
RUN dotnet build "LabWebApplication.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LabWebApplication.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LabWebApplication.dll"]
