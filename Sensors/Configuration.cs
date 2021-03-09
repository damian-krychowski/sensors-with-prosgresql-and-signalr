using System;
using System.IO;
using System.Security.Permissions;
using Microsoft.Extensions.Configuration;

namespace Sensors
{
    public static class Configuration
    {
        public static class Sections
        {
            public const string ConnectionString = "PostgreSQL:ConnectionString";
            public const string SensorsHubUrl = "SensorsHub:Url";
        }

        public static IConfiguration Read()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
        }
    }
}