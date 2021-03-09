using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Sensors
{
    public class SensorDataConsumer
    {
        private readonly int _consumerId;
        private readonly ChannelReader<SensorDataDto> _reader;
        private readonly Database _database;

        public SensorDataConsumer(
            int consumerId,
            ChannelReader<SensorDataDto> reader,
            Database database)
        {
            _consumerId = consumerId;
            _reader = reader;
            _database = database;
        }

        public Task StartConsuming(CancellationToken cancellationToken)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($"Consumer with id: {_consumerId} started");

                try
                {
                    while (await _reader.WaitToReadAsync(cancellationToken))
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }

                        if (_reader.TryRead(out var item))
                        {
                            await ProcessSensorData(item);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Stopping consumer with id: {_consumerId}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Consumer with id: {_consumerId} encountered unexpected error.");
                    Console.WriteLine(e);

                    throw;
                }

               
            }, cancellationToken);
        }

        private async Task ProcessSensorData(SensorDataDto sensorData)
        {
            Console.WriteLine($"Consumer with id: {_consumerId} " +
                              $"received SensorData with id: {sensorData.Id}");

            var entity = new SensorDataEntity
            {
                Id = sensorData.Id,
                Timestamp = sensorData.Timestamp,
                Data = sensorData.Data
            };

            await _database.InsertSensorData(entity);
        }
    }
}