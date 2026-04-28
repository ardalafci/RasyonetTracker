# 1. Çalıştırma ortamı (Uygulamanın üzerinde koşacağı temel sistem)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# 2. Derleme ortamı (Kodlarımızı derleyecek SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyasını kopyala ve paketleri indir
COPY ["RasyonetTracker.csproj", "./"]
RUN dotnet restore "RasyonetTracker.csproj"

# Tüm kodları kopyala ve derle
COPY . .
RUN dotnet build "RasyonetTracker.csproj" -c Release -o /app/build

# 3. Yayınlama (Publish - Sadece çalışması için gereken dosyaları ayırma)
FROM build AS publish
RUN dotnet publish "RasyonetTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Son İmaj (Final - En hafif ve temiz hali)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Uygulamayı ayağa kaldır
ENTRYPOINT ["dotnet", "RasyonetTracker.dll"]