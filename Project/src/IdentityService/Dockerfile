FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Project.sln ./
COPY src/IdentityService/IdentityService.csproj src/IdentityService/
RUN dotnet restore ./src/IdentityService/IdentityService.csproj

COPY . .
RUN dotnet publish ./src/IdentityService/IdentityService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 👇 Add this block to install vsdbg
RUN apt-get update && \
  apt-get install -y unzip curl && \
  curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /vsdbg

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT [ "dotnet", "IdentityService.dll" ]