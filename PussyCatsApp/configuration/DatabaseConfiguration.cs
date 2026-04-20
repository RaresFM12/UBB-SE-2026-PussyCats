using System;
using Microsoft.Extensions.Configuration;

namespace PussyCatsApp.Configuration
{
    public static class DatabaseConfiguration
    {
        private static string cachedConnectionString;

        public static string GetConnectionString()
        {
            if (cachedConnectionString == null)
            {
                cachedConnectionString = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build()
                    .GetConnectionString("raresConnectionString");
            }
            return cachedConnectionString;
        }
    }
}