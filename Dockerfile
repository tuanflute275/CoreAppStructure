# Sử dụng hình ảnh .NET 8 ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5095

# Sử dụng hình ảnh .NET 8 SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép solution file và project file để thực hiện restore
COPY CoreAppStructure.sln . 
COPY CoreAppStructure/*.csproj ./CoreAppStructure/
RUN dotnet restore CoreAppStructure.sln

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY CoreAppStructure/. ./CoreAppStructure/
WORKDIR /src/CoreAppStructure
RUN dotnet build CoreAppStructure.sln -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish /CoreAppStructure/CoreAppStructure.sln -c Release -o /app/publish

# Tạo giai đoạn final từ runtime base
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreAppStructure.dll"]


# docker build -t core-app:1.0.0 -f ./Dockerfile .
# docker tag coreapp-asp:1.0.0 tuanflute/core-asp-api:1.0.0
# docker push tuanflute/core-asp-api:1.0.0