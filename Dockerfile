# Sử dụng image .NET SDK để xây dựng ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# Sao chép mã nguồn từ GitHub vào container
WORKDIR /app
COPY . .
RUN apt-get update && apt-get install -y git

# Xây dựng ứng dụng
RUN dotnet restore
RUN dotnet build --configuration Release --no-restore

# Chạy các lệnh cần thiết để cài đặt và chạy ứng dụng
RUN dotnet publish --configuration Release --output /app/publish --no-restore

# Sử dụng image runtime nhẹ để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

# Sao chép ứng dụng đã được xây dựng sang image runtime
WORKDIR /app
COPY --from=build /app/publish .

# Mở cổng cho ứng dụng (nếu cần)
EXPOSE 80

# Chạy ứng dụng khi container được khởi chạy
ENTRYPOINT ["dotnet", "sample-vodka.dll"]