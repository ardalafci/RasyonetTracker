using System.Text.Json;
using RasyonetTracker.Models;
using RasyonetTracker.Repositories;
using RasyonetTracker.DTOs;

namespace RasyonetTracker.Services
{
    public class StockService
    {
        private readonly IStockRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // Dependency Injection ile Repository, HttpClient ve Configuration(appsettings) alıyoruz
        public StockService(IStockRepository repository, HttpClient httpClient, IConfiguration configuration)
        {
            _repository = repository;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<StockDto?> AddOrUpdateStockFromApiAsync(string symbol)
        {
            var apiKey = _configuration["Finnhub:ApiKey"];
            // Finnhub Hisse Fiyatı (Quote) Endpoint'i
            var url = $"https://finnhub.io/api/v1/quote?symbol={symbol.ToUpper()}&token={apiKey}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var content = await response.Content.ReadAsStringAsync();
            var quote = JsonSerializer.Deserialize<FinnhubQuoteResponse>(content);

            // Finnhub'da hisse bulunamazsa veya fiyat 0 dönerse null dönüyoruz
            if (quote == null || quote.c == 0) return null;

            var existingStock = await _repository.GetBySymbolAsync(symbol);

            // Veritabanında yoksa Ekle, varsa Güncelle
            if (existingStock == null)
            {
                existingStock = new Stock
                {
                    Symbol = symbol.ToUpper(),
                    CurrentPrice = quote.c,
                    LastUpdated = DateTime.UtcNow
                };
                await _repository.AddAsync(existingStock);
            }
            else
            {
                existingStock.CurrentPrice = quote.c;
                existingStock.LastUpdated = DateTime.UtcNow;
                await _repository.UpdateAsync(existingStock);
            }

            await _repository.SaveChangesAsync();

            // Client'a Entity Model yerine temiz DTO dönüyoruz
            return new StockDto
            {
                Symbol = existingStock.Symbol,
                CurrentPrice = existingStock.CurrentPrice,
                LastUpdated = existingStock.LastUpdated
            };
        }

        // İSTENEN ZORUNLU GÖREV: Analitik / Aggregation View
        // Portföydeki izlenen hisselerin ortalama değerini hesaplar
        public async Task<decimal> GetAveragePortfolioValueAsync()
        {
            var stocks = await _repository.GetAllAsync();
            if (!stocks.Any()) return 0;

            return stocks.Average(s => s.CurrentPrice);
        }

        // İkinci Analitik Görevi: Portföyün Toplam Değerini Hesaplar
        public async Task<decimal> GetTotalPortfolioValueAsync()
        {
            var stocks = await _repository.GetAllAsync();
            if (!stocks.Any()) return 0;

            // Bütün hisselerin o anki fiyatlarını toplar (Aggregation)
            return stocks.Sum(s => s.CurrentPrice);
        }
    }
}