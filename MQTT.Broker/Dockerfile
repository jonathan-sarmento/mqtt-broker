FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR ./MQTT.Broker
EXPOSE 80
EXPOSE 1883

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./*.csproj ./
RUN true
COPY . .	
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR ./
COPY --from=build ./MQTT.Broker/out .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet MQTT.Broker.dll
