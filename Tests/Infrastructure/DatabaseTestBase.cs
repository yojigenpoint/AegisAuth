using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using YojigenPoint.AegisAuth.Infrastructure.Persistence;

namespace Tests.Infrastructure
{
    public abstract class DatabaseTestBase : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly AppDbContext DbContext;
        protected readonly DbContextOptions<AppDbContext> Options;

        protected DatabaseTestBase()
        {
            // Create and open an in-memory SQLite database connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            Options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the DbContext and ensure the database is created
            DbContext = new AppDbContext(Options);
            DbContext.Database.EnsureCreated();
        }
        public void Dispose()
        {
            // Clean up database resources here
            _connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
