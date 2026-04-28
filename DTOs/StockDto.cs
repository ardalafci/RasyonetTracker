namespace RasyonetTracker.DTOs
{
    public class StockDto
    {
        public string Symbol { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}