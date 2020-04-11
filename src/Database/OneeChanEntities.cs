using Microsoft.EntityFrameworkCore;
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
            optionsBuilder.UseSqlite("Data Source=oneechan.db");
        }
    }
}