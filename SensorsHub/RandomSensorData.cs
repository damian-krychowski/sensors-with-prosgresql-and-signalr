using System;
using System.Linq;

namespace SensorsHub
{
    public static class RandomSensorData
    {
        private static readonly Random Random = new Random();

        public static object Create()
        {
            var sensorsCount = Random.Next(1, 151);

            var sensors = Enumerable
                .Range(1, sensorsCount)
                .ToDictionary(
                    sensorNum => $"sensor{sensorNum}",
                    _ => RandomizeSensorValue());

            return new
            {
                Id = Guid.NewGuid(),
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Data = sensors
            };
        }

        private static object RandomizeSensorValue()
        {
            var shouldUseDouble = RandomBool();

            if (shouldUseDouble)
            {
                return Random.NextDouble();
            }

            return RandomBool()
                ? "open"
                : "closed";
        }

        private static bool RandomBool()
        {
            return Random.Next(0, 2) == 1;
        }
    }
}