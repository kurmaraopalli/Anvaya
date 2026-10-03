FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln .
COPY src/Anvaya.Core/*.csproj ./src/Anvaya.Core/
COPY src/Anvaya.Infrastructure/*.csproj ./src/Anvaya.Infrastructure/
COPY src/Anvaya.Api/*.csproj ./src/Anvaya.Api/
COPY tests/Anvaya.Tests/*.csproj ./tests/Anvaya.Tests/

# Restore dependencies
RUN dotnet restore

# Copy source code and build
COPY . .
WORKDIR /app/src/Anvaya.Api
RUN dotnet publish -c Release -o /out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /out .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Anvaya.Api.dll"]
