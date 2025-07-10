using MyMicroservice.Models;
using Microsoft.EntityFrameworkCore;

namespace MyMicroservice.Data
{
    public class CashCardDbContext : DbContext
    {
        public CashCardDbContext(DbContextOptions<CashCardDbContext> options)
            : base(options)
        {
        }
        public DbSet<CashCard> CashCards { get; set; }
    }
}