FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

RUN dotnet restore InvestmentTracker.API.csproj
RUN dotnet publish InvestmentTracker.API.csproj -c Release -o /app/publish

FROM build AS migrate
WORKDIR /src

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet ef database update \
    --project InvestmentTracker.API.csproj \
    --startup-project InvestmentTracker.API.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "InvestmentTracker.API.dll"]
