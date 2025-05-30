using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NoLooseCent.Models;

namespace NoLooseCent.DbContexts
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Income> Incomes { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Currency>().HasData(
                new Currency { Id = 1, Name = "USD Cash", Code = "USD-CASH" },
                new Currency { Id = 2, Name = "USD EcoCash", Code = "USD-ECO" },
                new Currency { Id = 3, Name = "USD InnBucks", Code = "USD-INN" },
                new Currency { Id = 4, Name = "Zimbabwean Dollar", Code = "ZWL" }
            );
        }
    }
}