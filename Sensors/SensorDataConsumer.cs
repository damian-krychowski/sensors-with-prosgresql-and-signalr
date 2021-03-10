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
                        if (_reader.TryRead(out var item))
                        {
                            await ProcessSensorData(
                                item,
                                cancellationToken);
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

        private async Task ProcessSensorData(
            SensorDataDto sensorData,
            CancellationToken cancellationToken)
        {
            Console.WriteLine($"Consumer with id: {_consumerId} " +
                              $"received SensorData with id: {sensorData.Id}");

            if (sensorData.Type != "SensorData")
            {
                Console.WriteLine(
                    $"Type '{sensorData.Type}' is not supported yet. " +
                    $"Only 'SensorData' can be processed. " +
                    $"Message will be ignored.");

                return;
            }

            
            var entity = new SensorDataEntity
            {
                Id = sensorData.Id,
                Timestamp = sensorData.Timestamp,
                Data = FilterSensorsData(sensorData.Data)
            };

            await _database.InsertSensorData(
                entity,
                cancellationToken);
        }

        private static Dictionary<string, object> FilterSensorsData(
            Dictionary<string, object> sensorData)
        {
            //todo: implement filtering logic which will only allow keys like 
            //todo: 'sensor___' to pass through (sensor1 - sensor150)
            //todo: all values which do not match this criteria should be filtered out 
            //todo: and maybe logged only so that we see that situations like this happens
            return sensorData;
        }
    }
}