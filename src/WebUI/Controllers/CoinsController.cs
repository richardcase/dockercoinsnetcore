using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using WebUI.Models;
using Serilog;
using Microsoft.Extensions.Logging;

namespace WebUI.Controllers
{
    [Route("api/coins")]
    [Produces("application/json")]
    public class CoinsController : Controller
    {
        private CacheSettings settings;
        private ILogger<CoinsController> logger;

        public CoinsController(IOptions<CacheSettings> cacheSettings, ILogger<CoinsController> logger)
        {
            settings = cacheSettings.Value;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                DateTimeOffset offset = new DateTimeOffset(DateTime.Now);

                var redisConnection = OpenConnection(settings.CacheAddress, settings.CachePassword);
                IDatabase db = redisConnection.GetDatabase();
                string hashes = db.StringGet("hashes");

                CoinsSummary summary = new CoinsSummary
                {
                    Coins = string.Empty,
                    Hashes = Convert.ToInt32(hashes),
                    Now = offset.ToUnixTimeMilliseconds()
                };

                return Json(summary);
            }
            catch (Exception ex)
            {
                logger.LogError("Error getting coins summary", ex.Message);
                return StatusCode(500);
            }
            
        }

        private ConnectionMultiplexer OpenConnection(string cacheAddress, string cachePassword)
        {
            ConfigurationOptions redisConfig = new ConfigurationOptions
            {
                EndPoints =
                {
                    { cacheAddress }
                },
                AbortOnConnectFail = false,
                Password = cachePassword,
                Ssl = true
            };

            return ConnectionMultiplexer.Connect(redisConfig);
        }
    }
}
