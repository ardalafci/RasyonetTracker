using Moq;
using RasyonetTracker.Models;
using RasyonetTracker.Repositories;
using RasyonetTracker.Services;
using Microsoft.Extensions.Configuration;

namespace RasyonetTracker.Tests
{
    public class StockServiceTests
    {
        // Test 1: Portföyde hisseler varken ortalama fiyatı doğru hesaplamalı
        [Fact]
        public async Task GetAveragePortfolioValueAsync_ShouldCalculateCorrectAverage()
        {
            // Arrange (Hazırlık)
            var mockRepo = new Mock<IStockRepository>();
            var fakeStocks = new List<Stock>
            {
                new Stock { Symbol = "AAPL", CurrentPrice = 150 },
                new Stock { Symbol = "TSLA", CurrentPrice = 250 }
            };

            // "GetAllAsync çağrıldığında gerçek veritabanına gitme, benim sahte listemi dön" diyoruz. (Mocking)
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(fakeStocks);

            // Gerekli olmayan HttpClient ve IConfiguration için de sahte nesneler (dummy) veriyoruz
            var mockHttp = new HttpClient();
            var mockConfig = new Mock<IConfiguration>();

            var service = new StockService(mockRepo.Object, mockHttp, mockConfig.Object);

            // Act (Eylem)
            var result = await service.GetAveragePortfolioValueAsync();

            // Assert (Doğrulama)
            // (150 + 250) / 2 = 200 dönmesini bekliyoruz.
            Assert.Equal(200, result);
        }

        // Test 2: Portföy boşken hata fırlatmamalı, 0 dönmeli
        [Fact]
        public async Task GetAveragePortfolioValueAsync_WhenPortfolioEmpty_ShouldReturnZero()
        {
            // Arrange
            var mockRepo = new Mock<IStockRepository>();
            mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<Stock>());

            var service = new StockService(mockRepo.Object, new HttpClient(), new Mock<IConfiguration>().Object);

            // Act
            var result = await service.GetAveragePortfolioValueAsync();

            // Assert
            Assert.Equal(0, result);
        }
    }
}