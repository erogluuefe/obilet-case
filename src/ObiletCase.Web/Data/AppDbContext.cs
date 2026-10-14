using Microsoft.EntityFrameworkCore;
using ObiletCase.Web.Localization;

namespace ObiletCase.Web.Data;

/// <summary>The application's database. The provider is chosen in Program.cs, so this class does not depend on SQLite.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Translation> Translations => Set<Translation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Translation>(translation =>
        {
            // SQLite ignores these lengths, but databases such as PostgreSQL enforce them.
            translation.Property(t => t.Key).HasMaxLength(100).IsRequired();
            translation.Property(t => t.Culture).HasMaxLength(10).IsRequired();
            translation.Property(t => t.Value).HasMaxLength(500).IsRequired();

            // A text can only have one translation per language.
            translation.HasIndex(t => new { t.Key, t.Culture }).IsUnique();

            translation.HasData(TranslationSeed.Translations);
        });
    }
}
