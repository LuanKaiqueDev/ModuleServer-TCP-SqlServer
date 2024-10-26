using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Character?> Characters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Character>().HasData(new[]
        {
            new Character { Id = 1, Name = "Luan", ShipId = 1 },
            new Character { Id = 2, Name = "Marcrake", ShipId = 1 }
        });
        base.OnModelCreating(modelBuilder);
    }
}