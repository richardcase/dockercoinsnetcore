using System;

namespace WebUI
{
    public class CacheSettings
    {
        public string CacheAddress { get; set; }

        public string CachePassword { get; set; }

        public static CacheSettings CreateFromEnvironmentSettings()
        {
            return new CacheSettings
            {
                CacheAddress = Environment.GetEnvironmentVariable("DOCKERCOINS_CACHE_ADDR"),
                CachePassword = Environment.GetEnvironmentVariable("DOCKERCOINS_CACHE_PWD")
            };

        }
    }
}
