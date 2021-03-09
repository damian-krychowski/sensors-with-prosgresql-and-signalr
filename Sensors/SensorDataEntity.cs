using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Extensions.Configuration;

namespace Sensors
{
    [Table("sensor_data")]
    public class SensorDataEntity
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("timestamp")]
        public long Timestamp { get; set; }

        [Column("data", TypeName = "jsonb")]
        public Dictionary<string,object> Data { get; set; }
    }
}