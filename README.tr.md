# 📈 RasyonetTracker - Finansal Veri REST API

## 🚀 Proje Özeti ve Mimari Kararlar

Bu proje, Rasyonet Yazılım Mühendisliği Staj programı teknik değerlendirmesi için geliştirilmiş; canlı hisse senedi verilerini çeken ve portföy yönetimini SQLite üzerinde gerçekleştiren modern bir **.NET 8 Web API** çözümüdür.

Değerlendirme yönergelerinde de vurgulandığı gibi, tek bir "doğru" çözüm olmadığı için birincil odak noktam **temiz mimari (clean architecture), sürdürülebilirlik ve net karar alma süreçleri** oldu. Uygulamayı istenen temel gereksinimlerin ötesine taşıyıp endüstri standartlarına uygun hale getirmek için şu mimari kararları aldım:

* **Sorumlulukların Ayrılığı (Separation of Concerns):** İş mantığını (business logic - `Services`) HTTP yönlendirmelerinden (`Controllers`) kesin bir çizgiyle ayırdım. Controller sınıflarını olabildiğince ince tuttum.
* **Repository Deseni & Test Edilebilirlik:** Veri erişim katmanını (`IStockRepository`) soyutladım. Bu sayede iş mantığını Entity Framework bağımlılığından kurtardım ve kod tabanını **xUnit ve Moq** ile son derece test edilebilir hale getirdim.
* **Pürüzsüz Geliştirici Deneyimi (Frictionless Setup):** Projeyi değerlendirecek ekibin kodları herhangi bir .NET SDK veya veritabanı kurulumu ile uğraşmadan anında test edebilmesi için uygulamayı **Dockerize** ettim. Tüm sistem tek komutla ayağa kalkabilmektedir.

## 📡 Veri Kaynağı (Data Source) ve Entegrasyon

Projede dış veri kaynağı olarak sunulan seçenekler arasından **Finnhub API**'yi tercih ettim. 

**Neden Finnhub?**
* **Limit Avantajı:** Finnhub'ın ücretsiz katmanda sunduğu *dakikada 60 istek (60 req/min)* limiti, günde sadece 25 istek hakkı veren Alpha Vantage'a kıyasla hem geliştirme hem de test süreçlerinde çok daha esnek ve rahat bir çalışma alanı sağladı.
* **Amaca Uygunluk:** Projenin temel amacı olan "canlı hisse senedi fiyatlarını takip etme" (real-time quote) senaryosu için Finnhub'ın uç noktaları (endpoints) oldukça sade ve yeterliydi.

**Entegrasyon ve Güvenlik:**
Dış API entegrasyonu, mimariyi kirletmemesi adına sadece `StockService` içerisinde `HttpClient` kullanılarak izole bir şekilde gerçekleştirildi. Güvenlik en iyi uygulamaları (best practices) gereği, API anahtarı (API Key) kesinlikle kaynak kodun (source code) içine sabitlenmedi (hardcoded). Bunun yerine konfigürasyon üzerinden (`appsettings.json` veya Ortam Değişkenleri - Environment Variables) okunacak şekilde yapılandırıldı.


## ✅ Gereksinimlerin Karşılanma Durumu (Requirements Breakdown)

Proje geliştirilirken mülakat dokümanında belirtilen tüm zorunlu, gizli ve bonus maddeler titizlikle incelenmiş ve aşağıdaki şekilde projelendirilmiştir:

### 🔴 Zorunlu İsterler (Must Have)
* **.NET 6 veya üzeri:** Proje, güncel LTS sürümü olan **.NET 8** ile geliştirilmiştir.
* **Dış API Entegrasyonu:** Gerçek zamanlı fiyat verileri için **Finnhub API** kullanılarak entegrasyon sağlanmıştır.
* **Analitik / Agregasyon Senaryosu:** Veritabanındaki hisselerin o anki canlı fiyatları üzerinden portföyün **toplam değerini (Total Value)** ve **genel ortalamasını (Average Value)** hesaplayan analitik uç noktalar yazılmıştır.
* **Veritabanı:** Kurulum kolaylığı ve platform bağımsızlığı açısından **SQLite** (Entity Framework Core ile) tercih edilmiştir. Temel varlık (Entity) olarak `Stock` modeli kullanılmıştır.
* **Temiz RESTful API:** 4 adet temel CRUD işlemi ve 1 adet agregasyon olmak üzere toplam 5 temiz uç nokta (endpoint) oluşturulmuştur.
* **OOP Prensipleri:** Proje genelinde Interface (Arayüz) kullanımı, Encapsulation (Kapsülleme) ve Dependency Injection (Bağımlılık Enjeksiyonu) gibi OOP ilkelerine sıkı sıkıya bağlı kalınmıştır.
* **Tasarım Deseni (Design Pattern):** Veri erişim katmanını soyutlamak amacıyla **Repository Pattern** kullanılmıştır. Dokümanda istenildiği üzere, ilgili sınıfların içerisine bu deseni neden seçtiğimi açıklayan yorum satırları eklenmiştir. İhtiyaç dışı desenleri zorlamaktan ("forcing patterns unnecessarily") kaçınılmıştır.
* **Swagger:** Geliştirme ortamında (Development) tamamen aktif ve fonksiyoneldir.
* **README.md:** Şu an okuduğunuz bu doküman, istenen tüm bilgileri (amacı, seçimleri, talimatları) içerecek şekilde hazırlanmıştır.
* **Hatasız Derlenme:** Proje hiçbir hata almadan derlenmekte ve çalışmaktadır.

### 🟡 Gizli Değerlendirilenler (Evaluated During Code Review)
* **Hata Yönetimi (Error Handling):** İstemciye ham exception fırlatılmamış; try-catch blokları ile sarmalanarak 404 Not Found, 400 Bad Request, 201 Created ve 200 OK gibi anlamlı HTTP durum kodları (status codes) dönülmüştür.
* **Kod Kalitesi:** Temiz isimlendirme (Clean Code) standartlarına uyulmuş ve ölü kodlar bırakılmamıştır.
* **Proje Yapısı ve Katmanlar:** Proje; `Controllers`, `Services`, `Repositories`, `Models` ve `DTOs` klasörleri ile mantıksal katmanlara ayrılmıştır.
* **Sorumlulukların Ayrılığı (Separation of Concerns):** İş mantığı (Business Logic) kesinlikle Controller içerisinde yazılmamış, `StockService` içerisine taşınmıştır.
* **Git Geçmişi:** Proje tek bir commit ile yüklenmek yerine; "feat:", "test:", "chore:" gibi isimlendirme standartlarıyla, aşama aşama ve anlamlı Git commit'leri ile işlenmiştir.

### 🟢 Bonus Özellikler (Bonus)
* **⭐ Birim Testleri (Unit Tests):** Kodun sadece kalitesine odaklanılmış ("quality over quantity"); temel iş mantığını barındıran `StockService`, **xUnit** ve **Moq** kütüphaneleri kullanılarak test edilmiştir.
* **🐳 Docker Desteği:** Proje kök dizinine `Dockerfile` ve `.dockerignore` dosyaları eklenerek uygulama konteynerize edilmiştir. Herhangi bir SDK gereksinimi olmadan tek bir Docker komutu ile ayağa kalkabilmektedir.
* **Neden Frontend Yapılmadı?:** Pozisyonun "Backend Development" odaklı olması ve kısıtlı zaman faktörü göz önüne alındığında; basit bir arayüz yazmak yerine, backend'in mimarisine (Repository Pattern), güvenilirliğine (xUnit Tests) ve dağıtım kolaylığına (Docker) odaklanmanın daha profesyonel bir yaklaşım olacağı değerlendirilmiştir.
* **Neden Ekstra Tasarım Deseni Eklenmedi?:** Yönergelerdeki *"Tasarım desenlerini gereksiz yere zorlamayın"* uyarısı dikkate alınmıştır. Projenin ölçeği göz önünde bulundurulduğunda Repository Pattern mimariyi yeterince soyutlamıştır; Factory veya CQRS gibi desenler bu boyuttaki bir uygulama için "over-engineering" (aşırı mühendislik) olacağından bilinçli olarak kullanılmamıştır.

## 🐳 Docker Kurulum ve Çalıştırma Talimatları (Frictionless Setup)

Değerlendirme sürecini en pürüzsüz hale getirmek ve ortam bağımlılıklarını ortadan kaldırmak için bu proje tamamen konteynerize edilmiştir. Uygulamayı yerel makinenizde test etmek için **.NET SDK veya SQLite kurmanıza gerek yoktur.** Sadece Docker'ın kurulu ve çalışıyor olması yeterlidir.

Aşağıdaki adımları izleyerek projeyi saniyeler içinde izole bir ortamda ayağa kaldırabilirsiniz:

### 1. Docker İmajını Derleme (Build)
Terminalinizi projenin kök dizininde (yani `Dockerfile` dosyasının bulunduğu ana klasörde) açın ve projeyi paketlemek için şu komutu çalıştırın:
```bash
docker build -t rasyonettracker-api .
```

### 2. Konteyneri Başlatma (Run)
```bash
docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name myapi rasyonettracker-api
```

### 3. API'ye Erişim

Konteyner başarıyla başlatıldıktan sonra, uygulamayı test etmek için tarayıcınızı açın ve aşağıdaki adrese gidin:

**http://localhost:8080/swagger**