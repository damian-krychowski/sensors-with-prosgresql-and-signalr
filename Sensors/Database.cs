using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sensors
{
    public class Database
    {
        private readonly string _connectionString;

        public Database(IConfiguration configuration)
        {
            _connectionString = configuration
                .GetSection(Configuration.Sections.ConnectionString)
                .Value;
        }

        public async Task InsertSensorData(
            SensorDataEntity entity,
            CancellationToken cancellationToken)
        {
            await using var dbContext = new SensorsContext(
                _connectionString);

            await dbContext.AddAsync(
                entity,
                cancellationToken);

            await dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }
}