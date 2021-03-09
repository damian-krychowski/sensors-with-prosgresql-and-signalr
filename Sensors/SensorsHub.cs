using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Sensors
{
    public class SensorsHub
    {
        private readonly SensorDataProducer _sensorDataProducer;
        private HubConnection _connection;

        public SensorsHub(SensorDataProducer sensorDataProducer)
        {
            _sensorDataProducer = sensorDataProducer;
        }

        public async Task StartToListen()
        {
            var _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:64480/sensorshub")
                .Build();

            _connection.On<SensorDataDto>("SensorData", sensorData =>
            {
                Console.WriteLine($"New SensorData received (id: {sensorData.Id})");

                _sensorDataProducer.Produce(sensorData);
            });

            await _connection.StartAsync();
        }

        public async Task StopToListen()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
            }
        }
    }
}