using Microsoft.EntityFrameworkCore;

namespace Sensors
{
    public class SensorsContext : DbContext
    {
        private readonly string _connectionString;

        public SensorsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<SensorDataEntity> SensorData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(_connectionString);
    }
}