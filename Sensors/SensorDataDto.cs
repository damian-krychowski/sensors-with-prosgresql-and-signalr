using System;
using System.Collections.Generic;

namespace Sensors
{
    public class SensorDataDto
    {
        public Guid Id { get; set; }

        public long Timestamp { get; set; }

        public Dictionary<string, object> Data { get; set; }
    }
}