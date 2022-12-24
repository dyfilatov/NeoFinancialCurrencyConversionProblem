FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NeoFinancialCurrencyConversionProblem/NeoFinancialCurrencyConversionProblem.csproj", "NeoFinancialCurrencyConversionProblem/"]
RUN dotnet restore "NeoFinancialCurrencyConversionProblem/NeoFinancialCurrencyConversionProblem.csproj"
COPY . .
WORKDIR "/src/NeoFinancialCurrencyConversionProblem"
RUN dotnet build "NeoFinancialCurrencyConversionProblem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NeoFinancialCurrencyConversionProblem.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NeoFinancialCurrencyConversionProblem.dll"]
