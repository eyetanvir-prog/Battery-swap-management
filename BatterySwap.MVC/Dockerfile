FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BatterySwap.MVC/BatterySwap.MVC.csproj", "BatterySwap.MVC/"]
COPY ["BatterySwap.Shared/BatterySwap.Shared.csproj", "BatterySwap.Shared/"]
RUN dotnet restore "BatterySwap.MVC/BatterySwap.MVC.csproj"

COPY . .
WORKDIR "/src/BatterySwap.MVC"
RUN dotnet publish "BatterySwap.MVC.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update \
    && apt-get install -y curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80
ENTRYPOINT ["dotnet", "BatterySwap.MVC.dll"]
