using Microsoft.EntityFrameworkCore;
using RasyonetTracker.Data;
using RasyonetTracker.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// 1. SQLite Veritabanını sisteme tanıtıyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Repository Pattern'i sisteme enjekte ediyoruz. 
builder.Services.AddScoped<IStockRepository, StockRepository>();

// ---------------------------------------------------------

var app = builder.Build();

// OTOMATİK VERİTABANI OLUŞTURMA (Migration derdinden kurtarır)
// Proje ayağa kalkarken tracker.db dosyası yoksa otomatik oluşturur. 
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}
// -------------------------------------------------------------------

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