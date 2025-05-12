using Microsoft.EntityFrameworkCore;

namespace CryptoCurrency.Models;

public class CryptoContext : DbContext
{
    public CryptoContext(DbContextOptions<CryptoContext> options) : base(options) { }

    public DbSet<Crypto> Cryptos { get; set; }
    public DbSet<DollarRate> DollarRates { get; set; }
   
}
