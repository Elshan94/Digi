using DigitalSalaryService.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DigitalSalaryService.Persistence
{
    public class DigitalSalaryDbContext : DbContext
    {
        public DigitalSalaryDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }


        public DbSet<SalaryOrder> SalaryOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DigitalSalaryDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
