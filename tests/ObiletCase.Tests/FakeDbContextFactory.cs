using Microsoft.EntityFrameworkCore;
using ObiletCase.Web.Data;

namespace ObiletCase.Tests;

/// <summary>Stands in for the real DbContext factory: hands out contexts that use the options of the test database.</summary>
public class FakeDbContextFactory : IDbContextFactory<AppDbContext>
{
    private readonly DbContextOptions<AppDbContext> _options;

    public FakeDbContextFactory(DbContextOptions<AppDbContext> options)
    {
        _options = options;
    }

    public AppDbContext CreateDbContext()
    {
        return new AppDbContext(_options);
    }
}
