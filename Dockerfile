# Sử dụng image SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy file .csproj và restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ code và build
COPY . ./
RUN dotnet publish -c Release -o out

# Sử dụng image runtime để chạy
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose port (mặc định 8080 cho Render, Azure, hoặc môi trường Docker)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Lệnh chạy ứng dụng
ENTRYPOINT ["dotnet", "CoffeeHouse.dll"]