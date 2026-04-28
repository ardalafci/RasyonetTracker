using Microsoft.EntityFrameworkCore;
using RasyonetTracker.Data;
using RasyonetTracker.Repositories;
using RasyonetTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Veritabanı Konfigürasyonu (SQLite'ı appsettings.json'daki yola bağlıyoruz)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repository Pattern Bağımlılık Enjeksiyonu
builder.Services.AddScoped<IStockRepository, StockRepository>();

// 3. Dış API'lere (Finnhub) istek atmak için HttpClient'i ekliyoruz
builder.Services.AddHttpClient();

// 4. İş mantığımızı yürüten Service katmanını ekliyoruz
builder.Services.AddScoped<StockService>();

var app = builder.Build();

// 5. OTOMATİK VERİTABANI OLUŞTURMA
// Proje ayağa kalkarken tracker.db dosyası yoksa otomatik oluşturur.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();