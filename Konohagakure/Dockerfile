#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0-jammy-arm64v8 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0-jammy-arm64v8 AS build
WORKDIR /src
COPY ["Konohagakure/Konohagakure.csproj", "Konohagakure/"]
RUN dotnet restore "./Konohagakure/./Konohagakure.csproj"
COPY . .
WORKDIR "/src/Konohagakure"
RUN dotnet build "./Konohagakure.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./Konohagakure.csproj" -c Release -o /app/publish -r linux-arm

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Konohagakure.dll"]