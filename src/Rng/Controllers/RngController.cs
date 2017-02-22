using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;

namespace Rng.Controllers
{
    [Route("api/rng")]
    public class RngController : Controller
    {

        // GET api/rng/32
        [HttpGet("{length}")]
        public string Get(int length)
        {
            var gen = RandomNumberGenerator.Create();
            var buffer = new byte[length];
            gen.GetBytes(buffer);

            long generatedNumber = BitConverter.ToInt64(buffer, 0);

            return generatedNumber.ToString();
        }
    }
}
