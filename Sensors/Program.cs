using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sensors
{
    class Program
    {
        private const int ParallelConsumersCount = 15;

        static void Main(string[] args)
        {
            var configuration = Configuration.Read();

            var database = new Database(
                configuration);

            var channel = Channel.CreateUnbounded<SensorDataDto>();

            var consumers = PrepareConsumers(
                channel, 
                database,
                ParallelConsumersCount);

            var producer = new SensorDataProducer(
                channel.Writer);

            var sensorsHub = new SensorsHub(
                producer);

            var cancellationToken = new CancellationTokenSource();

            var processingJob = StartProcessing(
                consumers, 
                cancellationToken, 
                sensorsHub);

            Console.WriteLine("Press any key to to exit.");
            Console.ReadLine();

            StopClient(
                cancellationToken, 
                sensorsHub,
                producer,
                processingJob, 
                channel);
        }

        private static Task StartProcessing(
            SensorDataConsumer[] consumers, 
            CancellationTokenSource cancellationToken,
            SensorsHub sensorsHub)
        {
            var consumptionJobs = consumers
                .Select(c => c.StartConsuming(cancellationToken.Token))
                .ToArray();

            sensorsHub.StartToListen().Wait();

            return Task.WhenAll(consumptionJobs);
        }

        private static SensorDataConsumer[] PrepareConsumers(
            Channel<SensorDataDto> channel, 
            Database database,
            int consumersCount)
        {
            return Enumerable
                .Range(1, consumersCount)
                .Select(x => new SensorDataConsumer(
                    x,
                    channel.Reader,
                    database))
                .ToArray();
        }

        private static void StopClient(
            CancellationTokenSource cancellationToken, 
            SensorsHub sensorsHub,
            SensorDataProducer producer, 
            Task processingJob, 
            Channel<SensorDataDto> channel)
        {
            cancellationToken.Cancel();

            sensorsHub.StopToListen().Wait();
            producer.Complete();


            Console.WriteLine("Sensor client closing...");

            processingJob.Wait();
            channel.Reader.Completion.Wait();

            Console.WriteLine("Sensor client closed...");
        }
    }
}
