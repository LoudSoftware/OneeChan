using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;
using OneeChan.Database.Entities;

namespace OneeChan.Database
{
    class OneeChanEntities : DbContext
    {
        public virtual DbSet<Guild> Guilds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // var connectionStringBuilder = new SqliteConnectionStringBuilder
            // {
            //     DataSource = "oneechan.db",
            //
            // };
            // var connectionString = connectionStringBuilder.ToString();
            // var connection = new SqliteConnection(connectionString);

            // optionsBuilder.UseSqlite(connection);
            // optionsBuilder.UseSqlite("Data Source=oneechan.db");
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=OneeChan;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

    }
}