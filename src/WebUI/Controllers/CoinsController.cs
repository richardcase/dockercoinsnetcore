using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebUI.Models;

namespace WebUI.Controllers
{
    [Route("api/coins")]
    [Produces("application/json")]
    public class CoinsController : Controller
    {
        [HttpGet]
        public JsonResult Get()
        {
            //TODO: get value from redis
            //val, err:= coinsCache.GetInt("hashes")

            DateTimeOffset offset = new DateTimeOffset(DateTime.Now);


            CoinsSummary summary = new CoinsSummary
            {
                Coins = string.Empty,
                Hashes = 55,
                Now = offset.ToUnixTimeSeconds()
            };

            return Json(summary);
        }
    }
}
