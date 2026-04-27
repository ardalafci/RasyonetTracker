using Microsoft.EntityFrameworkCore;
using RasyonetTracker.Data;
using RasyonetTracker.Models;

namespace RasyonetTracker.Repositories
{
    /* * DESIGN PATTERN: Repository Pattern
     * Neden kullanıldı?: Veritabanı erişim mantığını (Entity Framework Core) is kurallarından (Business Logic) ayırmak için kullanıldı. 
     * Bu sayede Controller ve Service katmanlarımız veritabanının nasil çalistigini bilmek zorunda kalmaz. 
     * Kodun test edilebilirliğini artirir ve ileride SQLite'tan baska bir veritabanina gecersek sadece bu sinifi degistirmemiz yeterli olur.
     */
    public class StockRepository : IStockRepository
    {
        private readonly AppDbContext _context;

        public StockRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Stock>> GetAllAsync()
        {
            return await _context.Stocks.ToListAsync();
        }

        public async Task<Stock?> GetBySymbolAsync(string symbol)
        {
            // Case-insensitive (büyük/küçük harf duyarsız) arama yapıyoruz. AAPL ile aapl aynı sayılsın diye.
            return await _context.Stocks
                .FirstOrDefaultAsync(s => s.Symbol.ToUpper() == symbol.ToUpper());
        }

        public async Task AddAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
        }

        public Task UpdateAsync(Stock stock)
        {
            _context.Stocks.Update(stock);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}