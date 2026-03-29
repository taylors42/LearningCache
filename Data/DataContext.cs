using LearningCache.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningCache.Data;

public class DataContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseInMemoryDatabase("database");
    }
}