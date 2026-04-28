using RasyonetTracker.Models; 
namespace RasyonetTracker.Repositories
{
    /* * * MİMARİ KARAR: Neden Interface Kullanıldı?
     * 1. Sözleşme (Contract): Service katmanına sadece "ne" yapılabileceğini söyler, "nasıl" yapılacağını gizler.
     * 2. Loosely Coupled (Gevşek Bağlılık): Sistemi belirli bir veritabanı teknolojisine (örneğin EF Core) sıkı sıkıya bağlamaz.
     * 3. Test Edilebilirlik: Unit Test'ler yazılırken gerçek veritabanı yerine bu arayüz üzerinden sahte (Mock) veriler gönderilmesini sağlar.
     */
    public interface IStockRepository
    {
        Task<IEnumerable<Stock>> GetAllAsync();
        Task<Stock?> GetBySymbolAsync(string symbol);
        Task AddAsync(Stock stock);
        Task UpdateAsync(Stock stock);
        Task SaveChangesAsync();
    }
}