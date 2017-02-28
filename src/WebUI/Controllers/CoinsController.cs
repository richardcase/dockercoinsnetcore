using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
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
                    Now = offset.ToUnixTimeSeconds()
                };

                return Json(summary);
            }
            catch (Exception ex)
            {
                logger.LogError("Error getting coins summary", ex.ToString());
                return StatusCode(500);
            }
            
        }

        private ConnectionMultiplexer OpenConnection(string cacheAddress, string cachePassword)
        {
            string redisIp = GetIPFromHostName(cacheAddress);

            ConfigurationOptions redisConfig = new ConfigurationOptions
            {
                EndPoints =
                {
                    { redisIp }
                },
                AbortOnConnectFail = false,
                Password = cachePassword,
                Ssl = true
            };
            redisConfig.CertificateValidation += (sender, cert, chain, errors) => true;

            return ConnectionMultiplexer.Connect(redisConfig);
        }

        private string GetIPFromHostName(string hostName)
        {

            string[] s = hostName.Split(':');
            string host = s[0];
            int port = Convert.ToInt32(s[1]);

            if (IsIpAddress(host))
            {
                return hostName;
            }

            var ip = Dns.GetHostEntryAsync(host).GetAwaiter().GetResult();
            return $"{ip.AddressList.First(x => IsIpAddress(x.ToString()))}:{port}";
        }

        private static bool IsIpAddress(string host)
        {
            return Regex.IsMatch(host, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
        }
    }
}
