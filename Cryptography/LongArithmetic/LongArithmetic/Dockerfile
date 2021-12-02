FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LongArithmetic/LongArithmetic.csproj", "LongArithmetic/"]
RUN dotnet restore "LongArithmetic/LongArithmetic.csproj"
COPY . .
WORKDIR "/src/LongArithmetic"
RUN dotnet build "LongArithmetic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LongArithmetic.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LongArithmetic.dll"]
