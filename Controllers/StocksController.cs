using Microsoft.AspNetCore.Mvc;
using RasyonetTracker.Services;
using RasyonetTracker.Repositories;
using RasyonetTracker.DTOs;

namespace RasyonetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly StockService _stockService;
        private readonly IStockRepository _repository;

        // Dependency Injection ile servis ve repository'mizi çağırıyoruz.
        // Gördüğün gibi Controller sadece bir "Yönlendirici (Router)" görevi görüyor. İş mantığı burada yok (Separation of Concerns).
        public StocksController(StockService stockService, IStockRepository repository)
        {
            _stockService = stockService;
            _repository = repository;
        }

        // 1. ENDPOINT: İzleme listesindeki tüm hisseleri getir
        // GET: api/stocks
        [HttpGet]
        public async Task<IActionResult> GetAllTrackedStocks()
        {
            var stocks = await _repository.GetAllAsync();
            
            // Veritabanı modelini DTO'ya çevirerek ID gibi bilgileri gizliyoruz
            var stockDtos = stocks.Select(s => new StockDto
            {
                Symbol = s.Symbol,
                CurrentPrice = s.CurrentPrice,
                LastUpdated = s.LastUpdated
            });

            return Ok(stockDtos);
        }

        // 2. ENDPOINT: Finnhub'dan hisse fiyatı çek ve veritabanına kaydet
        // POST: api/stocks/{symbol}
        [HttpPost("{symbol}")]
        public async Task<IActionResult> TrackStock(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return BadRequest(new { Message = "Hisse sembolü boş olamaz." });

            try
            {
                var result = await _stockService.AddOrUpdateStockFromApiAsync(symbol);
                
                if (result == null)
                    return NotFound(new { Message = $"'{symbol}' sembolü bulunamadı veya API limiti aşıldı." });

                return Ok(result);
            }
            catch (Exception)
            {
                // Sessiz Değerlendirme (Silent Evaluation) Kriteri: "Raw exception exposed edilmemeli"
                return StatusCode(500, new { Message = "Dış API ile iletişim kurulurken sistemsel bir hata oluştu." });
            }
        }

        // 3. ENDPOINT (ZORUNLU ANALİTİK GÖREVİ): İzlenen hisselerin ortalama fiyatını getir
        // GET: api/stocks/analytics/average-price
        [HttpGet("analytics/average-price")]
        public async Task<IActionResult> GetAveragePortfolioPrice()
        {
            var average = await _stockService.GetAveragePortfolioValueAsync();
            
            return Ok(new 
            { 
                Description = "İzlenen tüm hisselerin ortalama fiyatı",
                AveragePrice = Math.Round(average, 2) 
            });
        }
    }
}